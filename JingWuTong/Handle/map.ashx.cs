﻿using DbComponent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Policesystem.Handle
{
    /// <summary>
    /// map 的摘要说明
    /// </summary>
    public class map : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string requesttype = context.Request.Form["requesttype"];
            StringBuilder sqltext = new StringBuilder();
           switch (requesttype)
            {
                case "设备搜索":
                    goto 设备搜索;
                case "查询人员":
                    goto 查询人员;
                case "轨迹查询":
                    goto 轨迹查询;
                default:
                    break;
            }

        #region 选中设备类型为人员
        设备搜索:
            string ssdd = context.Request.Form["ssdd"];
            string sszd = context.Request.Form["sszd"];
            string search = context.Request.Form["search"];
            string type = context.Request.Form["type"];
            string status = context.Request.Form["status"];
            string searchcondition = "";
            if (type == "0") //人员
            {
                searchcondition = (search == "") ? " " : "  and(u.[JYBH] like '%" + search + "%' or u.XM like '%" + search + "%')";
                if (ssdd == "all")
                {
                    if (status == "all")
                    {
                        sqltext.Append("SELECT g.IsOnline, u.XM,d.DevType,u.BMDM,e.BMJC,g.OnlineTime,d.DevId,u.JYBH,u.sj as Tel FROM [ACL_USER] U LEFT JOIN Device d  on U.JYBH = d.JYBH LEFT JOIN Entity e  on d.BMDM = e.BMDM LEFT JOIN Gps g on g.PDAID = d.DevId WHERE e.BMJC  is NOT NULL");
                    }
                    else
                    {
                        sqltext.Append("SELECT  g.IsOnline, u.XM,d.DevType,u.BMDM,e.BMJC,g.OnlineTime,d.DevId,u.JYBH,u.sj as Tel  FROM [ACL_USER] U LEFT JOIN Device d  on U.JYBH = d.JYBH LEFT JOIN Entity e  on d.BMDM = e.BMDM LEFT JOIN Gps g on g.PDAID = d.DevId WHERE   g.IsOnline = " + status + " and e.BMJC is NOT NULL");

                    }
                    sqltext.Append(searchcondition + " ORDER BY u.JYBH");

                    goto end;
                }

                if (sszd == "all")
                {
                    if (status == "all")
                    {
                        sqltext.Append("WITH childtable(BMMC,BMDM,SJBM) as (SELECT BMMC,BMDM,SJBM FROM [Entity] WHERE SJBM= '" + ssdd + "' UNION ALL SELECT A.BMMC,A.BMDM,A.SJBM FROM [Entity] A,childtable b where a.SJBM = b.BMDM ) SELECT g.IsOnline, u.XM,d.DevType,u.BMDM,e.BMJC,g.OnlineTime,d.DevId,u.JYBH,u.sj as Tel  FROM [ACL_USER] U LEFT JOIN Device d  on U.JYBH = d.JYBH LEFT JOIN Entity e  on d.BMDM = e.BMDM LEFT JOIN Gps g on g.PDAID = d.DevId WHERE e.BMDM in (SELECT BMDM from childtable) and e.BMJC  is NOT NULL");
                    }
                    else
                    {
                        sqltext.Append("WITH childtable(BMMC,BMDM,SJBM) as (SELECT BMMC,BMDM,SJBM FROM [Entity] WHERE SJBM= '" + ssdd + "' UNION ALL SELECT A.BMMC,A.BMDM,A.SJBM FROM [Entity] A,childtable b where a.SJBM = b.BMDM ) SELECT  g.IsOnline, u.XM,d.DevType,u.BMDM,e.BMJC,g.OnlineTime,d.DevId,u.JYBH,u.sj as Tel  FROM [ACL_USER] U LEFT JOIN Device d  on U.JYBH = d.JYBH LEFT JOIN Entity e  on d.BMDM = e.BMDM LEFT JOIN Gps g on g.PDAID = d.DevId WHERE e.BMDM in (SELECT BMDM from childtable) and  g.IsOnline = " + status + " and e.BMJC is NOT NULL");

                    }
                    sqltext.Append(searchcondition + " ORDER BY u.JYBH");

                    goto end;
                }
              
               if (status == "all")
                 {
                     sqltext.Append("SELECT g.IsOnline, u.XM,d.DevType,u.BMDM,e.BMJC,g.OnlineTime,d.DevId,u.JYBH,u.sj as Tel  FROM [ACL_USER] U LEFT JOIN Device d  on U.JYBH = d.JYBH LEFT JOIN Entity e  on d.BMDM = e.BMDM LEFT JOIN Gps g on g.PDAID = d.DevId WHERE  e.BMDM ='" + sszd + "'  and e.BMJC  is NOT NULL");
                  }
                else
                {
                    sqltext.Append("SELECT  g.IsOnline, u.XM,d.DevType,u.BMDM,e.BMJC,g.OnlineTime,d.DevId,u.JYBH,u.sj as Tel  FROM [ACL_USER] U LEFT JOIN Device d  on U.JYBH = d.JYBH LEFT JOIN Entity e  on d.BMDM = e.BMDM LEFT JOIN Gps g on g.PDAID = d.DevId WHERE e.BMDM ='" + sszd+"' and  g.IsOnline = " + status + " and e.BMJC is NOT NULL");

                }
                sqltext.Append(searchcondition + " ORDER BY u.JYBH");

                goto end;


            }
            #endregion
            #region 选中设备类型为除人员以外的其它，对讲机、执法记录仪、警务通等8小件
            else
            {
                searchcondition = (search == "") ? " and d.DevType ='" + type + "'" : " and d.DevType ='" + type + "'" + "  and(d.IMEI like '%" + search + "%'  or d.DevId like '%" + search + "%' or u.XM like '%" + search + "%')";
                if (ssdd == "all")
                {
                    if (status == "all")
                    {
                        sqltext.Append("SELECT g.IsOnline, u.XM,d.DevType,u.BMDM,e.BMJC,g.OnlineTime,d.DevId,u.JYBH,u.sj as Tel  FROM Device d  LEFT JOIN [ACL_USER] U on U.JYBH = d.JYBH LEFT JOIN Entity e  on d.BMDM = e.BMDM LEFT JOIN Gps g on g.PDAID = d.DevId WHERE e.BMJC  is NOT NULL ");
                    }
                    else
                    {
                        sqltext.Append("SELECT  g.IsOnline, u.XM,d.DevType,u.BMDM,e.BMJC,g.OnlineTime,d.DevId,u.JYBH,u.sj as Tel  FROM Device d  LEFT JOIN [ACL_USER] U on U.JYBH = d.JYBH LEFT JOIN Entity e  on d.BMDM = e.BMDM LEFT JOIN Gps g on g.PDAID = d.DevId WHERE   g.IsOnline = " + status + " and e.BMJC is NOT NULL");

                    }
                    sqltext.Append(searchcondition + " ORDER BY u.JYBH");
                    goto end;
                }

                if (sszd == "all")
                {
                    if (status == "all")
                    {
                        sqltext.Append("WITH childtable(BMMC,BMDM,SJBM) as (SELECT BMMC,BMDM,SJBM FROM [Entity] WHERE SJBM= '" + ssdd + "' or BMDM= '" + ssdd + "' UNION ALL SELECT A.BMMC,A.BMDM,A.SJBM FROM [Entity] A,childtable b where a.SJBM = b.BMDM ) SELECT g.IsOnline, u.XM,d.DevType,u.BMDM,e.BMJC,g.OnlineTime,d.DevId,u.JYBH,u.sj as Tel  FROM Device d  LEFT JOIN [ACL_USER] U on U.JYBH = d.JYBH LEFT JOIN Entity e  on d.BMDM = e.BMDM LEFT JOIN Gps g on g.PDAID = d.DevId WHERE e.BMDM in (SELECT BMDM from childtable) and e.BMJC  is NOT NULL ");
                    }
                    else
                    {
                        sqltext.Append("WITH childtable(BMMC,BMDM,SJBM) as (SELECT BMMC,BMDM,SJBM FROM [Entity] WHERE SJBM= '" + ssdd + "' or BMDM= '" + ssdd + "' UNION ALL SELECT A.BMMC,A.BMDM,A.SJBM FROM [Entity] A,childtable b where a.SJBM = b.BMDM ) SELECT  g.IsOnline, u.XM,d.DevType,u.BMDM,e.BMJC,g.OnlineTime,d.DevId,u.JYBH,u.sj as Tel  FROM Device d  LEFT JOIN [ACL_USER] U on U.JYBH = d.JYBH LEFT JOIN Entity e  on d.BMDM = e.BMDM LEFT JOIN Gps g on g.PDAID = d.DevId WHERE e.BMDM in (SELECT BMDM from childtable) and  g.IsOnline = " + status + " and e.BMJC is NOT NULL ");

                    }
                    sqltext.Append(searchcondition + " ORDER BY u.JYBH");
                    goto end;
                }

                if (status == "all")
                {
                    sqltext.Append("WITH childtable(BMMC,BMDM,SJBM) as (SELECT BMMC,BMDM,SJBM FROM [Entity] WHERE SJBM= '" + ssdd + "' or BMDM= '" + ssdd + "' UNION ALL SELECT A.BMMC,A.BMDM,A.SJBM FROM [Entity] A,childtable b where a.SJBM = b.BMDM ) SELECT g.IsOnline, u.XM,d.DevType,u.BMDM,e.BMJC,g.OnlineTime,d.DevId,u.JYBH,u.sj as Tel  FROM Device d  LEFT JOIN [ACL_USER] U on U.JYBH = d.JYBH LEFT JOIN Entity e  on d.BMDM = e.BMDM LEFT JOIN Gps g on g.PDAID = d.DevId WHERE  e.BMDM in (SELECT BMDM from childtable)   and e.BMJC  is NOT NULL");
                }
                else
                {
                    sqltext.Append("WITH childtable(BMMC,BMDM,SJBM) as (SELECT BMMC,BMDM,SJBM FROM [Entity] WHERE SJBM= '" + ssdd + "' or BMDM= '" + ssdd + "' UNION ALL SELECT A.BMMC,A.BMDM,A.SJBM FROM [Entity] A,childtable b where a.SJBM = b.BMDM ) SELECT  g.IsOnline, u.XM,d.DevType,u.BMDM,e.BMJC,g.OnlineTime,d.DevId,u.JYBH,u.sj as Tel  FROM Device d  LEFT JOIN [ACL_USER] U on U.JYBH = d.JYBH LEFT JOIN Entity e  on d.BMDM = e.BMDM LEFT JOIN Gps g on g.PDAID = d.DevId WHERE e.BMDM in (SELECT BMDM from childtable)  and  g.IsOnline = " + status + " and e.BMJC is NOT NULL ");
                }
                sqltext.Append(searchcondition + " ORDER BY u.JYBH");
                goto end;
            }
          
        #endregion

          查询人员:
            string sbbh = context.Request.Form["sbbh"];
            string jybh = context.Request.Form["jybh"];
            sqltext.Append("SELECT u.XM,d.DevType,d.Devid FROM [ACL_USER] u FULL OUTER JOIN  Device d on u.JYBH = d.JYBH where u.JYBH ='" + jybh+ "' or d.DevId ='"+sbbh+ "' order by d.DevType");
          end:       

            DataTable dt = SQLHelper.ExecuteRead(CommandType.Text, sqltext.ToString(), "DB");
            context.Response.Write(JSON.DatatableToJson(dt, ""));
            return;

        轨迹查询:
            string sDate = context.Request.Form["date"];
            string Devid = context.Request.Form["deviid"];
            string detype = context.Request.Form["detype"];
            string[] sArray = Devid.Split(',');
            string[] dArray = detype.Split(',');
            StringBuilder json = new StringBuilder();
            DataTable dtt;
            json.Append("[");
            for (int i1 = 0; i1 < sArray.Length; i1++)
            {              
              
                if (i1 > 0)
                {
                    json.Append(',');
                };
                json.Append("{\"Name\":");
                json.Append('"');
                json.Append(sArray[i1]);
                json.Append('"');
                json.Append(',');
                json.Append("\"type\":");
                json.Append('"');
                json.Append(dArray[i1]);
                json.Append('"');
                json.Append(',');
               // searchcondition = "SELECT top 1000 lo,la,SendTime QQSJ FROM [HistoryGps" + sDate.Substring(2, 2) + sDate.Substring(5, 2) + "] where PDAID ='57620086'  order by QQSJ";
                searchcondition = "SELECT lo,la,SendTime QQSJ FROM [HistoryGps"+sDate.Substring(2,2)+ sDate.Substring(5, 2) + "] where PDAID ='" + sArray[i1] + "' and SendTime >= '" + sDate + " 00:00:00' and  SendTime <= '" + sDate + " 23:59:59' order by SendTime";
                dtt = SQLHelper.ExecuteRead(CommandType.Text, searchcondition.ToString(), "DB");
                json.Append(JSON.DatatableToJS(dtt, "").ToString());

            }
            json.Append("]");
            context.Response.Write(json.ToString());

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}