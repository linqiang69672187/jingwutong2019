﻿using DbComponent;
using GemBox.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;

namespace JingWuTong.Handle
{
    /// <summary>
    /// Timesharing_Reports 的摘要说明
    /// </summary>
    public class Timesharing_Reports : IHttpHandler
    {

        DataTable allEntitys = null;  //递归单位信息表
        List<dataStruct> tmpList = new List<dataStruct>();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try {
          
            string type = context.Request.Form["type"];
            string begintime = context.Request.Form["begintime"];
            string endtime = context.Request.Form["endtime"];
            //string hbbegintime = context.Request.Form["hbbegintime"];
            //string hbendtime = context.Request.Form["hbendtime"];
            string ssdd = context.Request.Form["ssdd"];
            string sszd = context.Request.Form["sszd"];
            string requesttype = context.Request.Form["requesttype"];
            string search = context.Request.Form["search"];
            string sreachcondi = "";

            int onlinevalue = int.Parse(context.Request.Form["onlinevalue"]) * 60;
            int usedvalue = int.Parse(context.Request.Form["usedvalue"]) * 60;


            if (search != "")
            {
                sreachcondi = " (de.[IMEI] like '%" + search + "%' or de.[DevId] like '%" + search + "%' or us.[XM] like '%" + search + "%' or us.[JYBH] like '%" + search + "%' ) and ";
            }

            string tmpDevid = "***华丽的区别符号";
            int tmpRows = 0;
            int tmpcxl = 0;
            int tmpcll = 0;
            int tmpzxsj = 0;
            DataTable dtEntity = null;  //单位信息表

            DataTable Alarm_EveryDayInfo = null; //每日告警
            DataTable dUser = null;
            //typetext: typetext, ssddtext: ssddtext, sszdtext: sszdtext,
            // endtime = "2017/9/14"; //测试使用
            string title = "";
            if (context.Request.Form["ssddtext"] == "全部")
            {
                title = "台州交警局";
            }
            else
            {
                title = context.Request.Form["ssddtext"];
            }

            if (context.Request.Form["sszdtext"] != "全部")
            {
                title += context.Request.Form["sszdtext"];
            }



            StringBuilder sqltext = new StringBuilder();

            DataTable dtreturns = new DataTable(); //返回数据表
            dtreturns.Columns.Add("cloum1");
            dtreturns.Columns.Add("cloum2");
            dtreturns.Columns.Add("cloum3");
            dtreturns.Columns.Add("cloum4");
            dtreturns.Columns.Add("cloum5");
            dtreturns.Columns.Add("cloum6" );
            dtreturns.Columns.Add("cloum7");
            dtreturns.Columns.Add("cloum8");
            dtreturns.Columns.Add("cloum9");
            dtreturns.Columns.Add("cloum10");
            dtreturns.Columns.Add("cloum11");
            dtreturns.Columns.Add("cloum12");
            dtreturns.Columns.Add("cloum13");
            dtreturns.Columns.Add("cloum14");
            dtreturns.Columns.Add("cloum15");
            dtreturns.Columns.Add("cloum16");
            dtreturns.Columns.Add("cloum17");


            dtreturns.Columns.Add("cloum18");
            dtreturns.Columns.Add("cloum19");
            dtreturns.Columns.Add("cloum20");
            dtreturns.Columns.Add("cloum21");
            dtreturns.Columns.Add("cloum22");
            dtreturns.Columns.Add("cloum23");
            dtreturns.Columns.Add("cloum24");
            dtreturns.Columns.Add("cloum25");
            dtreturns.Columns.Add("cloum26");
            dtreturns.Columns.Add("cloum27");
            dtreturns.Columns.Add("cloum28");
            dtreturns.Columns.Add("cloum29");
            dtreturns.Columns.Add("cloum30");
            dtreturns.Columns.Add("cloum31");
            dtreturns.Columns.Add("cloum32");




            int days = Convert.ToInt16(context.Request.Form["dates"]);
            int statusvalue = 10;  //正常参考值
            int zxstatusvalue = 30;//在线参考值

            int devicescount = 0;  //汇总设备总数

            double spdx = 0.0;  //汇总视频大小
          
            Int64 jwtzxsc = 0;  //警务通在线时长
            int hzusecount = 0;
            int wcxl = 0;   //无查询量设备数量
            int wcfl = 0;   //无处罚量设备数量
            int wsysb = 0;  //无使用设备数量
            int zxsb = 0; //在线设备
            string pxstring = "";
            string bmdm = ""; //汇总的部门代码
            int allstatu_device = 0;  //汇总使用率不为空数量
            string ddtitle;//大队标题


            //------------------------------------------执法记录仪



            int sbsyl0 = 0;//设备使用数量
            int sbsyl1 = 0;//设备使用数量
            int sbsyl2 = 0;//设备使用数量
            int sbsyl3 = 0;//设备使用数量
            int sbsyl4 = 0;//设备使用数量

            int sbwsyl0 = 0;//设备未使用数量
            int sbwsyl1 = 0;//设备未使用数量
            int sbwsyl2 = 0;//设备未使用数量
            int sbwsyl3 = 0;//设备未使用数量
            int sbwsyl4 = 0;//设备未使用数量

            double spsc0 = 0.0;//视频时长
            double spsc1 = 0.0;//视频时长
            double spsc2 = 0.0;//视频时长
            double spsc3 = 0.0;//视频时长
            double spsc4 = 0.0;//视频时长

            double spdx0 = 0.0;//视频大小
            double spdx1=0.0;//视频大小
            double spdx2=0.0;//视频大小
            double spdx3=0.0;//视频大小
            double spdx4 = 0.0;//视频大小


            double gfscl0 = 0.0;//规范上传率
            double gfscl1 = 0.0;//规范上传率
            double gfscl2 = 0.0;//规范上传率
            double gfscl3 = 0.0;//规范上传率
            double gfscl4 = 0.0;//规范上传率


            double scl0 = 0.0;//上传总数
            double scl1 = 0.0;//上传总数
            double scl2 = 0.0;//上传总数
            double scl3 = 0.0;//上传总数
            double scl4 = 0.0;//上传总数

            //double usagerate0 = 0.0;//设备使用率
            //double usagerate1= 0.0;//设备使用率
            //double usagerate2 = 0.0;//设备使用率
            //double usagerate3= 0.0;//设备使用率
            //double usagerate4 = 0.0;//设备使用率


            int rowcout0 = 0;//记录当前时间断有几条
            //int rowcout1 = 0;//记录当前时间断有几条
            //int rowcout2 = 0;//记录当前时间断有几条
            //int rowcout3 = 0;//记录当前时间断有几条
            //int rowcout4 = 0;//记录当前时间断有几条


//-------------------------------------------- 警务通辅警通
            int jys = 0;//警员数

           long cfs0 = 0;//处罚数
           long cfs1 = 0;//处罚数
           long cfs2 = 0;//处罚数
           long cfs3 = 0;//处罚数
           long cfs4 = 0;//处罚数


           double rjcf0 = 0.0;//人均处罚
           double rjcf1 = 0.0;//人均处罚
           double rjcf2 = 0.0;//人均处罚
           double rjcf3 = 0.0;//人均处罚
           double rjcf4 = 0.0;//人均处罚

           Int64 cxl0 = 0;  //汇总查询量
           Int64 cxl1 = 0;  //汇总查询量
           Int64 cxl2 = 0;  //汇总查询量
           Int64 cxl3 = 0;  //汇总查询量
           Int64 cxl4 = 0;  //汇总查询量

           double pjcf0 = 0;// 汇总设备平均处罚量
           double pjcf1 = 0;// 汇总设备平均处罚量
           double pjcf2 = 0;// 汇总设备平均处罚量
           double pjcf3 = 0;// 汇总设备平均处罚量
           double pjcf4 = 0;// 汇总设备平均处罚量


           int wcfl0 = 0;   //无处罚量设备数量
           int wcfl1 = 0;   //无处罚量设备数量
           int wcfl2 = 0;   //无处罚量设备数量
           int wcfl3 = 0;   //无处罚量设备数量
           int wcfl4 = 0;   //无处罚量设备数量

//-----------------------------------------------------对讲机
            double usagerate0 = 0;//设备使用率
            double usagerate1 = 0;//设备使用率  //执法记录仪也用
            double usagerate2 = 0;//设备使用率
            double usagerate3 = 0;//设备使用率
            double usagerate4 = 0;//设备使用率

            int status0 = 0;
            int status1 = 0;
            int status2 = 0;
            int status3= 0;
            int status4 = 0;

            double zxsc0 = 0.0;  //汇总在线时长
            double zxsc1 = 0.0;  //汇总在线时长
            double zxsc2 = 0.0;  //汇总在线时长
            double zxsc3 = 0.0;  //汇总在线时长
            double zxsc4 = 0.0;  //汇总在线时长


            statusvalue = days * usedvalue;//超过10分钟算使用
            zxstatusvalue = days * onlinevalue;//在线参考值



            allEntitys = SQLHelper.ExecuteRead(CommandType.Text, "SELECT BMDM,SJBM from [Entity] ", "11");

            //所有大队
            if (ssdd == "all")
            {
                bmdm = "331000000000";
                ddtitle = "台州交警局";

                switch (type)
                {
                    case "5":
                        Alarm_EveryDayInfo = SQLHelper.ExecuteRead(CommandType.Text, "SELECT en.BMDM, en.SJBM as [ParentID],us.XM as [Contacts],de.[DevId],ala.在线时长,ala.[AlarmType],ala.文件大小, Time,UploadCnt,GFUploadCnt from (" +
                        "    SELECT  [DevId],sum([VideLength]) as 在线时长,sum([FileSize]) as 文件大小,1 as AlarmType, substring(convert(varchar,[Time],120),12,5) as Time,sum(UploadCnt) as UploadCnt,sum(GFUploadCnt) as GFUploadCnt  from EveryDayInfo_ZFJLY_Hour   where  [Time] >='" + begintime + "' and [Time] <='" + endtime + " 23:59' group by  substring(convert(varchar,[Time],120),12,5),DevId ) " +
                        "as ala left join [Device] as de on de.[DevId] = ala.[DevId] left join [Entity] as en on en.[BMDM] = de.[BMDM]  left join ACL_USER as us on de.JYBH = us.JYBH  where " + sreachcondi + " de.[BMDM]<>''" , "Alarm_EveryDayInfo");
                        dtEntity = SQLHelper.ExecuteRead(CommandType.Text, "SELECT BMDM as ID,BMJC as Name,SJBM as ParentID,BMJB AS Depth from [Entity] a where [SJBM]  = '331000000000' and [BMJC] IS NOT NULL AND BMJC <> '' ORDER  BY CASE WHEN Sort IS NULL THEN 1 ELSE Sort END desc", "2");
                        break;
                    default:

                        Alarm_EveryDayInfo = SQLHelper.ExecuteRead(CommandType.Text, "SELECT en.BMDM,  en.SJBM as [ParentID],us.XM as [Contacts],de.[DevId],ala.在线时长,0 as 文件大小,ala.处理量,ala.查询量, Time,2 as AlarmType from (" + "select DevId,substring(convert(varchar,[Time],120),12,5) Time,sum(OnlineTime) as 在线时长,sum(Isnull(HandleCnt,0)) as 处理量,sum(Isnull(CXCnt,0)) as 查询量 from EverydayInfo_Hour  where  Time >='" + begintime + "' and Time  <='" + endtime + " 23:59' group by substring(convert(varchar,[Time],120),12,5),DevId ) as ala left join [Device] as de on de.[DevId] = ala.[DevId] left join [Entity] as en on en.[BMDM] = de.[BMDM]     left join ACL_USER as us on de.JYBH = us.JYBH  where " + sreachcondi + " de.[DevType]=" + type, "Alarm_EveryDayInfo");


                        dtEntity = SQLHelper.ExecuteRead(CommandType.Text, "SELECT BMDM as ID,BMJC as Name,SJBM as ParentID,BMJB AS Depth from [Entity] a where [SJBM]  = '331000000000' and [BMJC] IS NOT NULL AND BMJC <> '' AND BMDM <> '33100000000x' ORDER  BY CASE WHEN Sort IS NULL THEN 1 ELSE Sort END desc", "2");

                        break;
                }
       
                dUser = SQLHelper.ExecuteRead(CommandType.Text, "SELECT en.SJBM,us.BMDM FROM [dbo].[ACL_USER] us left join Entity en on us.BMDM = en.BMDM", "user");
            }
            else
            {
                if (sszd == "all")//所有中队
                {  
                    bmdm = ssdd;
                    ddtitle = context.Request.Form["ssddtext"];
                    switch (type)
                    {
                        case "5":
                            Alarm_EveryDayInfo = SQLHelper.ExecuteRead(CommandType.Text, "WITH childtable(BMMC,BMDM,SJBM) as (SELECT BMMC,BMDM,SJBM FROM [Entity] WHERE SJBM= '" + ssdd + "' OR BMDM = '" + ssdd + "' UNION ALL SELECT A.BMMC,A.BMDM,A.SJBM FROM [Entity] A,childtable b where a.SJBM = b.BMDM ) "+
                                "  SELECT en.BMDM, en.[SJBM] as ParentID,us.XM as [Contacts],de.[DevId],[AlarmType],ala.在线时长,ala.文件大小, Time,UploadCnt,GFUploadCnt  from (SELECT [DevId],sum([VideLength]) as 在线时长,sum([FileSize]) as 文件大小,1 as AlarmType,substring(convert(varchar,[Time],120),12,5) as Time,sum(UploadCnt) as UploadCnt,sum(GFUploadCnt) as GFUploadCnt  from EveryDayInfo_ZFJLY_Hour  where  [Time] >='" + begintime + "' and [Time] <='" + endtime + " 23:59' group by substring(convert(varchar,[Time],120),12,5),DevId) as ala " +
                                " left join [Device] as de on de.[DevId] = ala.[DevId] left join [Entity] as en on en.[BMDM] = de.[BMDM]   left join ACL_USER as us on de.JYBH = us.JYBH where " + sreachcondi + " de.[DevType]=" + type + " and de.BMDM in (select BMDM from childtable) ", "Alarm_EveryDayInfo");

                            dtEntity = SQLHelper.ExecuteRead(CommandType.Text, "SELECT BMDM as [ID] ,BMJC as [Name] ,SJBM as [ParentID],BMJB as [Depth] from [Entity] where [SJBM] ='" + ssdd + "' or [BMDM]='" + ssdd + "'    order BY CASE WHEN Sort IS NULL THEN 1 ELSE Sort END desc", "2");
                            dUser = SQLHelper.ExecuteRead(CommandType.Text, "WITH childtable(BMMC,BMDM,SJBM) as (SELECT BMMC,BMDM,SJBM FROM [Entity] WHERE SJBM= '" + ssdd + "' OR BMDM = '" + ssdd + "' UNION ALL SELECT A.BMMC,A.BMDM,A.SJBM FROM [Entity] A,childtable b where a.SJBM = b.BMDM ) SELECT en.SJBM,us.BMDM FROM [ACL_USER] us left join Entity en on us.BMDM = en.BMDM where en.BMDM in (select BMDM from childtable)", "user");

                            break;
                        default:
                            Alarm_EveryDayInfo = SQLHelper.ExecuteRead(CommandType.Text, "WITH childtable(BMMC,BMDM,SJBM) as (SELECT BMMC,BMDM,SJBM FROM [Entity] WHERE SJBM= '" + ssdd + "' OR BMDM = '" + ssdd + "' " +" UNION ALL "+" SELECT A.BMMC,A.BMDM,A.SJBM FROM [Entity] A,childtable b where a.SJBM = b.BMDM )"+" SELECT en.BMDM,  en.[SJBM] as ParentID,us.XM as [Contacts],de.[DevId],ala.在线时长,2 as AlarmType,0 as 文件大小,ala.查询量,ala.处理量, Time from " + "( select DevId,substring(convert(varchar,[Time],120),12,5) as Time,sum(OnlineTime) as 在线时长,sum(Isnull(HandleCnt,0)) as 处理量,sum(Isnull(CXCnt,0)) as 查询量 from EverydayInfo_Hour where    Time >='" + begintime + "' and Time <='" + endtime + " 23:59' group by substring(convert(varchar,[Time],120),12,5), DevId  ) as ala " + " left join [Device] as de on de.[DevId] = ala.[DevId] left join [Entity] as en on en.[BMDM] = de.[BMDM]    left join ACL_USER as us on de.JYBH = us.JYBH where " + sreachcondi + " de.[DevType]=" + type + " and de.BMDM in (select BMDM from childtable) ", "Alarm_EveryDayInfo");


                            dtEntity = SQLHelper.ExecuteRead(CommandType.Text, "SELECT BMDM as [ID] ,BMJC as [Name] ,SJBM as [ParentID],BMJB as [Depth] from [Entity] where [SJBM] ='" + ssdd + "' or [BMDM]='" + ssdd + "'   order BY CASE WHEN Sort IS NULL THEN 1 ELSE Sort END desc", "2");
                            dUser = SQLHelper.ExecuteRead(CommandType.Text, "WITH childtable(BMMC,BMDM,SJBM) as (SELECT BMMC,BMDM,SJBM FROM [Entity] WHERE SJBM= '" + ssdd + "' OR BMDM = '" + ssdd + "' UNION ALL SELECT A.BMMC,A.BMDM,A.SJBM FROM [Entity] A,childtable b where a.SJBM = b.BMDM ) SELECT en.SJBM,us.BMDM FROM [ACL_USER] us left join Entity en on us.BMDM = en.BMDM where en.BMDM in (select BMDM from childtable)", "user");

                            break;
                    }


                }
                else
                {
                    bmdm = sszd;//当前中队 个人
                    ddtitle = context.Request.Form["sszdtext"];
                    switch (type)
                    {
                        case "5":
                            Alarm_EveryDayInfo = SQLHelper.ExecuteRead(CommandType.Text, "  WITH childtable(BMMC,BMDM,SJBM) as (SELECT BMMC,BMDM,SJBM FROM [Entity] WHERE SJBM= '" + sszd + "' OR BMDM = '" + sszd + "' UNION ALL SELECT A.BMMC,A.BMDM,A.SJBM FROM [Entity] A,childtable b where a.SJBM = b.BMDM ) SELECT us.JYBH,en.BMDM, en.[SJBM] as ParentID,us.XM as [Contacts],de.[DevId],[AlarmType],ala.在线时长,ala.文件大小, Time,0 as 处理量,0 as 查询量 from ( SELECT [DevId],[VideLength] as 在线时长,[FileSize] as 文件大小,1 as AlarmType,substring(convert(varchar,[Time],120),12,5) as Time from EveryDayInfo_ZFJLY_Hour   where  [Time] >='" + begintime + "' and [Time] <='" + endtime + " 23:59'   ) as ala left join [Device] as de on de.[DevId] = ala.[DevId] left join [Entity] as en on en.[BMDM] = de.[BMDM]  left join ACL_USER as us on de.JYBH = us.JYBH  where " + sreachcondi + " de.[DevType]=" + type + " and en.BMDM  in (select BMDM from childtable) ", "Alarm_EveryDayInfo");
                            dtEntity = SQLHelper.ExecuteRead(CommandType.Text, "WITH childtable(BMMC,BMDM,SJBM) as (SELECT BMMC,BMDM,SJBM FROM [Entity] WHERE SJBM= '" + sszd + "' OR BMDM = '" + sszd + "' UNION ALL SELECT A.BMMC,A.BMDM,A.SJBM FROM [Entity] A,childtable b where a.SJBM = b.BMDM )  SELECT us.JYBH,en.BMDM, en.[SJBM] as ParentID,us.XM as [Contacts],de.[DevId] from ( SELECT [DevId]  from EveryDayInfo_ZFJLY_Hour   where  [Time] >='" + begintime + "' and [Time] <='" + endtime + "'   group by [DevId] ) as ala left join [Device] as de on de.[DevId] = ala.[DevId] left join [Entity] as en on en.[BMDM] = de.[BMDM]  left join ACL_USER as us on de.JYBH = us.JYBH  where " + sreachcondi + " de.[DevType]=" + type + " and en.BMDM in (select BMDM from childtable)", "Alarm_EveryDayInfo");

                            break;
                        default:
                            Alarm_EveryDayInfo = SQLHelper.ExecuteRead(CommandType.Text, "SELECT us.JYBH,en.BMDM, en.SJBM as [ParentID],us.XM as [Contacts],de.[DevId],ala.在线时长,2 as AlarmType,0 as 文件大小, Time,ala.处理量,ala.查询量 from  " + "(select DevId,substring(convert(varchar,[Time],120),12,5) as Time,OnlineTime as 在线时长,Isnull(HandleCnt,0) as 处理量,Isnull(CXCnt,0) as 查询量  from EverydayInfo_Hour  where   Time  >='" + begintime + "' and Time <='" + endtime + " 23:59'   ) as ala " + " left join [Device] as de on de.[DevId] = ala.[DevId] left join [Entity] as en on en.[BMDM] = de.[BMDM]  left join ACL_USER as us on de.JYBH = us.JYBH  where " + sreachcondi + " de.[DevType]=" + type + " and en.BMDM='" + sszd + "' ", "Alarm_EveryDayInfo");
                            dtEntity = SQLHelper.ExecuteRead(CommandType.Text, "SELECT us.JYBH,en.BMDM, en.SJBM as [ParentID],us.XM as [Contacts],de.[DevId] from  " + "(select DevId  from EverydayInfo_Hour  where Time  >='" + begintime + "' and Time <='" + endtime + "'   group by DevId ) as ala " + " left join [Device] as de on de.[DevId] = ala.[DevId] left join [Entity] as en on en.[BMDM] = de.[BMDM]  left join ACL_USER as us on de.JYBH = us.JYBH  where " + sreachcondi + " de.[DevType]=" + type + " and en.BMDM='" + sszd + "' ", "Alarm_EveryDayInfo");

                            break;
                    }

                   // dtEntity = SQLHelper.ExecuteRead(CommandType.Text, " SELECT B.xm,B.JYBH,A.BMDM as ID,BMJC as Name,SJBM as ParentID,BMJB AS Depth,de.DevId from [Entity] as A  left join ACL_USER  as B on A.BMDM=B.BMDM  left join Device as de on de.JYBH = B.JYBH where A.[BMDM]  = '" + sszd + "'", "2");


                }
            }

            List<string[]> arryList = new List<string[]>();

                foreach (var key in ConfigurationManager.AppSettings.AllKeys)
                {
                    if (!key.Contains("Time")) continue;
                    arryList.Add(ConfigurationManager.AppSettings[key].Split('-'));

                }


                #region//个人

                if (ssdd != "all" && sszd != "all")
                {


                    for (int i1 = 0; i1 < dtEntity.Rows.Count; i1++)
                    {

                        DataRow dr = dtreturns.NewRow();


                        Int64 在线时长 = 0;
                        Int64 处理量 = 0;
                        Int64 文件大小 = 0;
                        Int64 查询量 = 0;
                        int 无查询量 = 0;
                        int 无处罚量 = 0;
                        int 未使用 = 0;
                        int usercount = 0;
                        int 在线 = 0;
                        int status = 0;//设备使用正常、周1次，月4次，季度12次

                        dr["cloum1"] = dtEntity.Rows[i1]["Contacts"].ToString();
                        dr["cloum2"] = dtEntity.Rows[i1]["JYBH"].ToString();
                        dr["cloum3"] = dtEntity.Rows[i1]["DevId"].ToString();


                      
                        for (int i = 0; i < arryList.Count; i++)
                        {

                            int Ftime = int.Parse(arryList[i][0].Replace(":", ""));
                            int Stime = int.Parse(arryList[i][1].Replace(":", ""));

                            var rows = (from p in Alarm_EveryDayInfo.AsEnumerable()
                                        where p.Field<string>("DevId") == dtEntity.Rows[i1]["DevId"].ToString() && int.Parse(p.Field<string>("Time").Replace(":", "")) >= Ftime && int.Parse(p.Field<string>("Time").Replace(":", "")) < Stime
                                        group p by new
                                        {
                                            t1 = p.Field<string>("devid"),
                                            t2 = p.Field<int>("AlarmType")

                                        } into g
                                        select new dataStruct
                                        {
                                            文件大小 = g.Sum(p => p.Field<int>("文件大小")),
                                            AlarmType = g.Key.t2,
                                            DevId = g.Key.t1,
                                            HandleCnt = g.Sum(p => p.Field<int>("处理量")),
                                            CXCnt = g.Sum(p => p.Field<int>("查询量")),
                                            在线时长 = g.Sum(p => p.Field<int>("在线时长"))

                                        }).ToList<dataStruct>();

                            //queryrows = (from p in Data.AsEnumerable()
                            //             where strList.ToArray().Contains(p.Field<string>("BMDM")) && strList.ToArray().Contains(p.Field<string>("BMDM")) && int.Parse(p.Field<string>("Time").Replace(":", "")) >= Ftime && int.Parse(p.Field<string>("Time").Replace(":", "")) < Stime
                            //             group p by new
                            //             {
                            //                 t1 = p.Field<string>("devid")


                            //             } into g
                            //             select new dataStruct
                            //             {
                            //                 在线时长 = g.Sum(p => p.Field<int>("OnlineTime"))
                            //             }).ToList<dataStruct>();

                            //获得设备数量，及正常使用设备
                            tmpRows = 0;
                            tmpcxl = 0;
                            tmpcll = 0;
                            tmpzxsj = 0;
                            在线时长 = 0;
                            处理量 = 0;
                            文件大小 = 0;
                            查询量 = 0;
                            foreach (dataStruct item in rows)
                            {



                                switch (item.AlarmType.ToString())
                                {
                                    case "1":
                                        在线时长 += Convert.ToInt32(item.在线时长);
                                        tmpzxsj += Convert.ToInt32(item.在线时长);
                                        在线 += ((Convert.ToInt32(item.在线时长) - zxstatusvalue) > 0) ? 1 : 0;
                                        文件大小 += Convert.ToInt32(item.文件大小);

                                        break;
                                    case "2":
                                        在线时长 += Convert.ToInt32(item.在线时长);
                                        tmpzxsj += Convert.ToInt32(item.在线时长);

                                        处理量 += Convert.ToInt32(item.HandleCnt);
                                        查询量 += Convert.ToInt32(item.CXCnt);
                                        tmpcxl += Convert.ToInt32(item.CXCnt);
                                        tmpcll += Convert.ToInt32(item.HandleCnt);
                                        break;
                                }
                                if (item.DevId.ToString() != tmpDevid)
                                {
                                    tmpRows += 1;  //新设备ID不重复
                                    tmpDevid = item.DevId.ToString();
                                    status += (Convert.ToInt32(item.在线时长) - statusvalue > 0) ? 1 : 0;
                                    allstatu_device += (Convert.ToInt32(item.在线时长) - statusvalue > 0) ? 1 : 0;

                                    未使用 += ((tmpzxsj - statusvalue) <= 0) ? 1 : 0;
                                    在线 += ((tmpzxsj - zxstatusvalue) > 0) ? 1 : 0;

                                    无处罚量 += (tmpcll == 0) ? 1 : 0;
                                    无查询量 += (tmpcxl == 0) ? 1 : 0;
                                    tmpcll = 0;
                                    tmpcxl = 0;
                                    tmpzxsj = 0;

                                }




                            }//循环结束


                            tmpList.Clear();




                            int countdevices = tmpRows;
                            double deviceuse = Math.Round((double)status * 100 / (double)countdevices, 2);



                            //dr["cloum2"] = countdevices;
                            devicescount += countdevices;


                            switch (type)
                            {
                                case "4":
                                case "6":
                                    switch (i)
                                    {
                                        case 0:
                                            dr["cloum4"] = 处理量;
                                            dr["cloum5"] = 查询量;
                                            break;
                                        case 1:
                                            dr["cloum6"] = 处理量;
                                            dr["cloum7"] = 查询量;
                                            break;
                                        case 2:
                                            dr["cloum8"] = 处理量;
                                            dr["cloum9"] = 查询量;
                                            break;
                                        case 3:
                                            dr["cloum10"] = 处理量;
                                            dr["cloum11"] = 查询量;
                                            break;
                                        case 4:
                                            dr["cloum12"] = 处理量;
                                            dr["cloum13"] = 查询量;
                                            break;
                                    }

                                    break;
                                case "1":
                                case "2":
                                case "3":
                                    switch (i)
                                    {
                                        case 0:
                                            dr["cloum4"] = ((double)在线时长 / 3600).ToString("0.00"); ;//在线时长
                                            break;

                                        case 1:
                                            dr["cloum5"] = ((double)在线时长 / 3600).ToString("0.00"); ;//在线时长
                                            break;

                                        case 2:
                                            dr["cloum6"] = ((double)在线时长 / 3600).ToString("0.00"); ;//在线时长
                                            break;

                                        case 3:
                                            dr["cloum7"] = ((double)在线时长 / 3600).ToString("0.00"); ;//在线时长
                                            break;

                                        case 4:
                                            dr["cloum8"] = ((double)在线时长 / 3600).ToString("0.00"); ;//在线时长
                                            break;
                                    }

                                    break;
                                case "5"://执法记录仪
                                    switch (i)
                                    {
                                        case 0:
                                            dr["cloum4"] = ((double)在线时长 / 3600).ToString("0.00");//视频时长总和
                                            dr["cloum5"] = ((double)文件大小 / 1048576).ToString("0.00");//视频大小（GB）
                                            break;
                                        case 1:
                                            dr["cloum6"] = ((double)在线时长 / 3600).ToString("0.00");//视频时长总和
                                            dr["cloum7"] = ((double)文件大小 / 1048576).ToString("0.00");//视频大小（GB）
                                            break;
                                        case 2:
                                            dr["cloum8"] = ((double)在线时长 / 3600).ToString("0.00");//视频时长总和
                                            dr["cloum9"] = ((double)文件大小 / 1048576).ToString("0.00");//视频大小（GB）
                                            break;
                                        case 3:
                                            dr["cloum10"] = ((double)在线时长 / 3600).ToString("0.00");//视频时长总和
                                            dr["cloum11"] = ((double)文件大小 / 1048576).ToString("0.00");//视频大小（GB）
                                            break;
                                        case 4:
                                            dr["cloum12"] = ((double)在线时长 / 3600).ToString("0.00");//视频时长总和
                                            dr["cloum13"] = ((double)文件大小 / 1048576).ToString("0.00");//视频大小（GB）
                                            break;
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }

                        dtreturns.Rows.Add(dr);

                    }
                }

                #endregion
               
                #region//大队和中队
                else
                {
                    for (int i1 = 0; i1 < dtEntity.Rows.Count; i1++)
                    {
                        DataRow dr = dtreturns.NewRow();
                        //dr["cloum1"] = (i1 + 1).ToString(); ;

                        dr["cloum1"] = dtEntity.Rows[i1]["Name"].ToString();//部门

                        rowcout0 = dtEntity.Rows.Count;
                        //dr["cloum13"] = (i1 + 1);
                        Int64 在线时长 = 0;
                        Int64 处理量 = 0;
                        Int64 文件大小 = 0;
                        Int64 查询量 = 0;
                        int 无查询量 = 0;
                        int 无处罚量 = 0;
                        int 未使用 = 0;
                        int usercount = 0;
                        int 在线 = 0;
                        int 上传总数 = 0;
                        int 规范上传总数 = 0;
                        int status = 0;//设备使用正常、周1次，月4次，季度12次

                        int sumdevices = 0;//设备配发数


                        var entityids = GetSonID(dtEntity.Rows[i1]["ID"].ToString());
                        List<string> strList = new List<string>();

                        strList.Add(dtEntity.Rows[i1]["ID"].ToString());

                        if (!(ssdd != "all" && sszd == "all") || !(dtEntity.Rows[i1]["ParentID"].ToString() == "331000000000"))//排除所有中队
                        {
                            foreach (entityStruct item in entityids)
                            {
                                strList.Add(item.BMDM);
                            }
                        }


                        var userrows = from p in dUser.AsEnumerable()
                                       where strList.ToArray().Contains(p.Field<string>("BMDM"))
                                       select p;
                        usercount = userrows.Count();

                        int countdevices = 0;
                        for (int i = 0; i < arryList.Count; i++)
                        {

                            int Ftime = int.Parse(arryList[i][0].Replace(":", ""));
                            int Stime = int.Parse(arryList[i][1].Replace(":", ""));
                            List<dataStruct> rows = null;

                            if (type == "5")
                            {
                                rows = (from p in Alarm_EveryDayInfo.AsEnumerable()
                                        where strList.ToArray().Contains(p.Field<string>("BMDM")) && int.Parse(p.Field<string>("Time").Replace(":", "")) >= Ftime && int.Parse(p.Field<string>("Time").Replace(":", "")) < Stime
                                        group p by new
                                        {
                                            t1 = p.Field<string>("devid")
                                        } into g
                                        select new dataStruct
                                        {
                                            BMDM = dtEntity.Rows[i1]["ID"].ToString(),
                                            ParentID = dtEntity.Rows[i1]["ParentID"].ToString(),
                                            在线时长 = g.Sum(p => p.Field<int>("在线时长")),
                                            文件大小 = g.Sum(p => p.Field<int>("文件大小")),
                                            AlarmType = 1,
                                            DevId = g.Key.t1,
                                            UploadCnt = g.Sum(p => p.Field<int>("UploadCnt")),
                                            GFUploadCnt = g.Sum(p => p.Field<int>("GFUploadCnt"))



                                        }).ToList<dataStruct>();
                            }
                            else
                            {
                                try { 
                                rows = (from p in Alarm_EveryDayInfo.AsEnumerable()
                                        where strList.ToArray().Contains(p.Field<string>("BMDM")) && int.Parse(p.Field<string>("Time").Replace(":", "")) >= Ftime && int.Parse(p.Field<string>("Time").Replace(":", "")) < Stime
                                        group p by new
                                        {
                                            t1 = p.Field<string>("devid")
                                        } into g

                                        select new dataStruct
                                        {
                                            BMDM = dtEntity.Rows[i1]["ID"].ToString(),
                                            ParentID = dtEntity.Rows[i1]["ParentID"].ToString(),
                                            在线时长 = g.Sum(p => p.Field<int>("在线时长")),
                                            文件大小 = g.Sum(p => p.Field<int>("文件大小")),
                                            AlarmType = 2,
                                            DevId = g.Key.t1,
                                            HandleCnt = g.Sum(p => p.Field<int>("处理量")),
                                            CXCnt = g.Sum(p => p.Field<int>("查询量"))

                                        }).ToList<dataStruct>();
                                }
                                catch (Exception e) {

                                }


                            }




                            //获得设备数量，及正常使用设备
                            tmpRows = 0;
                            tmpcxl = 0;
                            tmpcll = 0;
                            tmpzxsj = 0;
                            上传总数 = 0;
                            规范上传总数 = 0;
                            无处罚量 = 0;
                            无查询量 = 0;
                            未使用 = 0;
                            status = 0;
                            在线时长 = 0;
                            文件大小 = 0;
                            处理量 = 0;
                            查询量 = 0;
                            foreach (dataStruct item in rows)
                            {

                                switch (item.AlarmType.ToString())
                                {
                                    case "1":
                                        在线时长 += Convert.ToInt32(item.在线时长);
                                        文件大小 += Convert.ToInt32(item.文件大小);
                                        tmpzxsj += Convert.ToInt32(item.在线时长);

                                        上传总数 += item.UploadCnt;
                                        规范上传总数 += item.GFUploadCnt;
                                        break;
                                    case "2":
                                        在线时长 += Convert.ToInt32(item.在线时长);
                                        tmpzxsj += Convert.ToInt32(item.在线时长);

                                        处理量 += Convert.ToInt32(item.HandleCnt);
                                        查询量 += Convert.ToInt32(item.CXCnt);
                                        tmpcxl += Convert.ToInt32(item.CXCnt);
                                        tmpcll += Convert.ToInt32(item.HandleCnt);


                                        break;

                                }
                                if (item.DevId.ToString() != tmpDevid)
                                {
                                    tmpRows += 1;  //新设备ID不重复
                                    tmpDevid = item.DevId.ToString();



                                    无处罚量 += (tmpcll == 0) ? 1 : 0;
                                    无查询量 += (tmpcxl == 0) ? 1 : 0;
                                    未使用 += ((tmpzxsj - statusvalue) <= 0) ? 1 : 0;
                                    在线 += ((tmpzxsj - zxstatusvalue) > 0) ? 1 : 0;
                                    status += (tmpzxsj - statusvalue > 0) ? 1 : 0;
                                    allstatu_device += (tmpzxsj - statusvalue > 0) ? 1 : 0;
                                    tmpcll = 0;
                                    tmpcxl = 0;
                                    tmpzxsj = 0;



                                }




                            }//循环结束


                            tmpList.Clear();




                            countdevices = (countdevices == 0) ? tmpRows : countdevices;
                            double deviceuse = Math.Round((double)status * 100 / (double)countdevices, 2);

                            sumdevices += (i == 0) ? tmpRows : 0;
                            dr["cloum2"] = sumdevices;//配发数

                            devicescount += (i == 0) ? countdevices : 0;


                            switch (type)
                            {
                                case "4"://警务通
                                case "6"://辅警通

                                    switch (i)
                                    {
                                        case 0:


                                            dr["cloum3"] = usercount; //辅警数
                                            jys += usercount;

                                            dr["cloum4"] = 处理量;//设备处罚量
                                            cfs0 += 处理量;
                                            dr["cloum5"] = (usercount == 0) ? 0 : Math.Round((double)处理量 / usercount, 2);//人均处罚量
                                            rjcf0 += (usercount == 0) ? 0 : Math.Round((double)处理量 / usercount, 2);
                                            dr["cloum6"] = 查询量;
                                            cxl0 += 查询量;
                                            dr["cloum7"] = (countdevices == 0) ? 0 : Math.Round((double)处理量 / countdevices, 2); ;//设备平均处罚量
                                            pjcf0 += (countdevices == 0) ? 0 : Math.Round((double)处理量 / countdevices, 2); ;//设备平均处罚量
                                            dr["cloum8"] = 无处罚量;//无处罚数的警务通（台）
                                            wcfl0 += 无处罚量;
                                            break;
                                        case 1:
                                            dr["cloum9"] = 处理量;//设备处罚量
                                            cfs1 += 处理量;
                                            dr["cloum10"] = (usercount == 0) ? 0 : Math.Round((double)处理量 / usercount, 2);//人均处罚量
                                            rjcf1 += (usercount == 0) ? 0 : Math.Round((double)处理量 / usercount, 2);
                                            dr["cloum11"] = 查询量;
                                            cxl1 += 查询量;
                                            dr["cloum12"] = (countdevices == 0) ? 0 : Math.Round((double)处理量 / countdevices, 2); ;//设备平均处罚量
                                            pjcf1 += (countdevices == 0) ? 0 : Math.Round((double)处理量 / countdevices, 2); ;//设备平均处罚量
                                            dr["cloum13"] = 无处罚量;//无处罚数的警务通（台）
                                            wcfl1 += 无处罚量;
                                            break;
                                        case 2:
                                            dr["cloum14"] = 处理量;//设备处罚量
                                            cfs2 += 处理量;
                                            dr["cloum15"] = (usercount == 0) ? 0 : Math.Round((double)处理量 / usercount, 2);//人均处罚量
                                            rjcf2 += (usercount == 0) ? 0 : Math.Round((double)处理量 / usercount, 2);
                                            dr["cloum16"] = 查询量;
                                            cxl2 += 查询量;
                                            dr["cloum17"] = (countdevices == 0) ? 0 : Math.Round((double)处理量 / countdevices, 2); ;//设备平均处罚量
                                            pjcf2 += (countdevices == 0) ? 0 : Math.Round((double)处理量 / countdevices, 2); ;//设备平均处罚量
                                            dr["cloum18"] = 无处罚量;//无处罚数的警务通（台）
                                            wcfl2 += 无处罚量;
                                            break;
                                        case 3:
                                            dr["cloum19"] = 处理量;//设备处罚量
                                            cfs3 += 处理量;
                                            dr["cloum20"] = (usercount == 0) ? 0 : Math.Round((double)处理量 / usercount, 2);//人均处罚量
                                            rjcf3 += (usercount == 0) ? 0 : Math.Round((double)处理量 / usercount, 2);
                                            dr["cloum21"] = 查询量;
                                            cxl3 += 查询量;
                                            dr["cloum22"] = (countdevices == 0) ? 0 : Math.Round((double)处理量 / countdevices, 2); ;//设备平均处罚量
                                            pjcf3 += (countdevices == 0) ? 0 : Math.Round((double)处理量 / countdevices, 2); ;//设备平均处罚量
                                            dr["cloum23"] = 无处罚量;//无处罚数的警务通（台）
                                            wcfl3 += 无处罚量;
                                            break;
                                        case 4:
                                            dr["cloum24"] = 处理量;//设备处罚量
                                            cfs4 += 处理量;
                                            dr["cloum25"] = (usercount == 0) ? 0 : Math.Round((double)处理量 / usercount, 2);//人均处罚量
                                            rjcf4 += (usercount == 0) ? 0 : Math.Round((double)处理量 / usercount, 2);
                                            dr["cloum26"] = 查询量;
                                            cxl4 += 查询量;
                                            dr["cloum27"] = (countdevices == 0) ? 0 : Math.Round((double)处理量 / countdevices, 2); ;//设备平均处罚量
                                            pjcf4 += (countdevices == 0) ? 0 : Math.Round((double)处理量 / countdevices, 2); ;//设备平均处罚量
                                            dr["cloum28"] = 无处罚量;//无处罚数的警务通（台）
                                            wcfl4 += 无处罚量;
                                            break;

                                    }


                                    break;
                                case "1"://车载视频
                                case "2"://对讲机
                                case "3"://拦截仪
                                    switch (i)
                                    {
                                        case 0:
                                            dr["cloum3"] = status;//设备使用数量
                                            status0 += status;//设备使用数量总数
                                            dr["cloum4"] = Math.Round((double)在线时长 / 3600,2);//在线时长
                                            zxsc0 += Math.Round((double)在线时长 / 3600,2);//在线时长汇总

                                            dr["cloum5"] = (countdevices != 0) ? (deviceuse) : 0.0;//设备使用率

                                            usagerate0 += (devicescount == 0) ? 0 : ((double)allstatu_device * 100 / devicescount);
                                            break;

                                        case 1:
                                            dr["cloum6"] = status;//设备使用数量
                                            status1 += status;//设备使用数量总数
                                            dr["cloum7"] = Math.Round((double)在线时长 / 3600,2); ;//在线时长
                                            zxsc1 += Math.Round((double)在线时长 / 3600,2);//在线时长汇总
                                            dr["cloum8"] = (countdevices != 0) ? (deviceuse) : 0.0;//设备使用率
                                            usagerate1 += (devicescount == 0) ? 0 : ((double)allstatu_device * 100 / devicescount);
                                            break;
                                        case 2:
                                            dr["cloum9"] = status;//设备使用数量
                                            status2 += status;//设备使用数量总数
                                            dr["cloum10"] = Math.Round((double)在线时长 / 3600,2); ;//在线时长
                                            zxsc2 += Math.Round((double)在线时长 / 3600,2);//在线时长汇总
                                            dr["cloum11"] = (countdevices != 0) ? (deviceuse) : 0.0;//设备使用率
                                            usagerate2 += (devicescount == 0) ? 0 : ((double)allstatu_device * 100 / devicescount);
                                            break;
                                        case 3:
                                            dr["cloum12"] = status;//设备使用数量
                                            status3 += status;//设备使用数量总数
                                            dr["cloum13"] = Math.Round((double)在线时长 / 3600,2) ;//在线时长
                                            zxsc3 += Math.Round((double)在线时长 / 3600,2);//在线时长汇总
                                            dr["cloum14"] = (countdevices != 0) ? (deviceuse) : 0.0;//设备使用率
                                            usagerate3 += (devicescount == 0) ? 0 : ((double)allstatu_device * 100 / devicescount);
                                            break;
                                        case 4:
                                            dr["cloum15"] = status;//设备使用数量
                                            status4 += status;//设备使用数量总数
                                            dr["cloum16"] = Math.Round((double)在线时长 / 3600,2) ;//在线时长
                                            zxsc4 += Math.Round((double)在线时长 / 3600,2);//在线时长汇总
                                            dr["cloum17"] = (countdevices != 0) ? (deviceuse) : 0.0;//设备使用率
                                            usagerate4 += (devicescount == 0) ? 0 : ((double)allstatu_device * 100 / devicescount);
                                            break;


                                    }


                                    break;
                                case "5"://执法记录仪
                                    switch (i)
                                    {
                                        case 0:
                                            dr["cloum3"] = status;//设备使用数量
                                            sbsyl0 += status;
                                            dr["cloum4"] = 未使用;//设备未使用数量
                                            sbwsyl0 += 未使用;
                                            dr["cloum5"] = Math.Round((double)在线时长 / 3600,2);//视频时长总和
                                            spsc0 += Math.Round((double)在线时长 / 3600,2);
                                            dr["cloum6"] = Math.Round((double)文件大小 / 1048576,2);//视频大小（GB）
                                            spdx0 += Math.Round((double)文件大小 / 1048576,2);
                                            dr["cloum7"] = (上传总数 == 0) ? "0.00" : ((double)规范上传总数 * 100 / 上传总数).ToString("0.00");//规范上传率
                                            gfscl0 += 规范上传总数;
                                            scl0 += 上传总数;
                                            dr["cloum8"] = (countdevices != 0) ? (deviceuse) : 0;//设备使用率

                                            usagerate0 += (devicescount == 0) ? 0 : ((double)allstatu_device * 100 / devicescount);


                                            //rowcout0 = rows.Count ;
                                            break;

                                        case 1:
                                            dr["cloum9"] = status;//设备使用数量
                                            sbsyl1 += status;
                                            dr["cloum10"] = 未使用;//设备未使用数量
                                            sbwsyl1 += 未使用;
                                            dr["cloum11"] = Math.Round((double)在线时长 / 3600,2);//视频时长总和
                                            spsc1 += Math.Round((double)在线时长 / 3600,2);
                                            dr["cloum12"] = Math.Round((double)文件大小 / 1048576,2);//视频大小（GB）
                                            spdx1 += Math.Round((double)文件大小 / 1048576,2);
                                            dr["cloum13"] = (上传总数 == 0) ? 0 : Math.Round((double)规范上传总数 * 100 / 上传总数,2);//规范上传率
                                                                                                                                  //gfscl1 += (上传总数 == 0) ? 0.00 : ((double)规范上传总数 / 上传总数);
                                            gfscl1 += 规范上传总数;
                                            scl1 += 上传总数;
                                            dr["cloum14"] = (countdevices != 0) ? (deviceuse) : 0;//设备使用率
                                            usagerate1 += (devicescount == 0) ? 0 : ((double)allstatu_device * 100 / devicescount);

                                            //rowcout1 = rows.Count;
                                            break;
                                        case 2:
                                            dr["cloum15"] = status;//设备使用数量
                                            sbsyl2 += status;
                                            dr["cloum16"] = 未使用;//设备未使用数量
                                            sbwsyl2 += 未使用;
                                            dr["cloum17"] = Math.Round((double)在线时长 / 3600,2);//视频时长总和
                                            spsc2 += Math.Round((double)在线时长 / 3600,2);
                                            dr["cloum18"] = Math.Round((double)文件大小 / 1048576,2);//视频大小（GB）
                                            spdx2 += Math.Round((double)文件大小 / 1048576,2);
                                            dr["cloum19"] = (上传总数 == 0) ? 0 : Math.Round((double)规范上传总数 * 100 / 上传总数,2);//规范上传率
                                                                                                                                  //  gfscl1 += (上传总数 == 0) ? 0.00 : ((double)规范上传总数 / 上传总数);
                                            gfscl2 += 规范上传总数;
                                            scl2 += 上传总数;
                                            dr["cloum20"] = (countdevices != 0) ? (deviceuse) : 0;//设备使用率
                                            usagerate2 += (devicescount == 0) ? 0 : ((double)allstatu_device * 100 / devicescount);
                                            //rowcout2 = rows.Count;
                                            break;
                                        case 3:
                                            dr["cloum21"] = status;//设备使用数量
                                            sbsyl3 += status;
                                            dr["cloum22"] = 未使用;//设备未使用数量
                                            sbwsyl3 += 未使用;
                                            dr["cloum23"] = Math.Round((double)在线时长 / 3600,2);//视频时长总和
                                            spsc3 += Math.Round((double)在线时长 / 3600,2);
                                            dr["cloum24"] = Math.Round((double)文件大小 / 1048576,2);//视频大小（GB）
                                            spdx3 += Math.Round((double)文件大小 / 1048576,2);
                                            dr["cloum25"] = (上传总数 == 0) ? 0 : Math.Round((double)规范上传总数 * 100 / 上传总数,2);//规范上传率
                                                                                                                                  //  gfscl1 += (上传总数 == 0) ? 0.00 : ((double)规范上传总数 / 上传总数);
                                            gfscl3 += 规范上传总数;
                                            scl3 += 上传总数;
                                            dr["cloum26"] = (countdevices != 0) ? (deviceuse) : 0;//设备使用率
                                            usagerate3 += (devicescount == 0) ? 0 : ((double)allstatu_device * 100 / devicescount);
                                            //rowcout3 = rows.Count;
                                            break;
                                        case 4:
                                            dr["cloum27"] = status;//设备使用数量
                                            sbsyl4 += status;
                                            dr["cloum28"] = 未使用;//设备未使用数量
                                            sbwsyl4 += 未使用;
                                            dr["cloum29"] = Math.Round((double)在线时长 / 3600,2);//视频时长总和
                                            spsc4 += Math.Round((double)在线时长 / 3600,2);
                                            dr["cloum30"] = Math.Round((double)文件大小 / 1048576,2);//视频大小（GB）
                                            spdx4 += Math.Round((double)文件大小 / 1048576,2);
                                            dr["cloum31"] = (上传总数 == 0) ? 0 : Math.Round((double)规范上传总数 * 100 / 上传总数,2);//规范上传率
                                                                                                                                  // gfscl1 += (上传总数 == 0) ? 0.00 : ((double)规范上传总数 / 上传总数);
                                            gfscl4 += 规范上传总数;
                                            scl4 += 上传总数;
                                            dr["cloum32"] = (countdevices != 0) ? (deviceuse) : 0;//设备使用率
                                            usagerate4 += (devicescount == 0) ? 0 : ((double)allstatu_device * 100 / devicescount);
                                            //rowcout4 = rows.Count;
                                            break;
                                    }


                                    break;
                                default:
                                    break;
                            }
                        }

                        dtreturns.Rows.Add(dr);
                    }
                }
                #endregion




                if (ssdd != "all" && sszd != "all")
                {
                }


                #region 大队和中队汇总
                else
                {

                DataRow dr2 = dtreturns.NewRow();

                dr2["cloum1"] = "汇总";//ddtitle;
                dr2["cloum2"] = devicescount;
                dr2["cloum3"] = jys;

                switch (type)
                {
                    case "4":
                    case "6":
                        dr2["cloum4"] = cfs0;
                        dr2["cloum5"] = (jys > 0) ? Math.Round(((double)cfs0 / jys),2) : 0;//rjcf0;
                        dr2["cloum6"] = cxl0;
                            // dr2["cloum7"] = pjcf0;
                        dr2["cloum7"] = (devicescount > 0) ? Math.Round(((double)cfs0 / devicescount),2):0;
                        dr2["cloum8"] = wcfl0;

                        dr2["cloum9"] = cfs1;
                        dr2["cloum10"] = (jys > 0) ? ((double)cfs1 / jys).ToString("0.00") : "0";//rjcf1;
                        dr2["cloum11"] = cxl1;
                        dr2["cloum12"] = (devicescount > 0) ? Math.Round(((double)cfs1 / devicescount),2) : 0;
                        dr2["cloum13"] = wcfl1;


                        dr2["cloum14"] = cfs2;
                        dr2["cloum15"] = (jys > 0) ? ((double)cfs2 / jys).ToString("0.00") : "0";//rjcf2;
                            dr2["cloum16"] = cxl2;
                        dr2["cloum17"] = (devicescount > 0) ? Math.Round(((double)cfs2 / devicescount),2) : 0;
                        dr2["cloum18"] = wcfl2;


                        dr2["cloum19"] = cfs3;
                        dr2["cloum20"] = (jys > 0) ? ((double)cfs3 / jys).ToString("0.00") : "0";//rjcf3;
                            dr2["cloum21"] = cxl3;
                        dr2["cloum22"] = (devicescount > 0) ? Math.Round(((double)cfs3 / devicescount),2) : 0;
                        dr2["cloum23"] = wcfl3;
                        dr2["cloum24"] = cfs4;
                        dr2["cloum25"] = (jys > 0) ? ((double)cfs4 / jys).ToString("0.00") : "0";//rjcf4;
                            dr2["cloum26"] = cxl4;
                        dr2["cloum27"] = (devicescount > 0) ? Math.Round(((double)cfs4 / devicescount),2) : 0;
                        dr2["cloum28"] = wcfl4;



                        break;
                    case "5":   //执法记录仪

                        dr2["cloum3"] = sbsyl0;
                        dr2["cloum4"] = sbwsyl0;
                        dr2["cloum5"] = spsc0.ToString("0.00");
                        dr2["cloum6"] = spdx0.ToString("0.00");                 
                        dr2["cloum7"] = (scl0 > 0) ? Math.Round((gfscl0*100 / scl0),2):0;
                        dr2["cloum8"] = (devicescount == 0) ? 0 : Math.Round(((double)sbsyl0 * 100 / devicescount), 2); ; //Math.Round(usagerate0, 2);//设备使用率汇总

                        dr2["cloum9"] = sbsyl1;
                        dr2["cloum10"] = sbwsyl1;
                        dr2["cloum11"] = spsc1.ToString("0.00");
                        dr2["cloum12"] = spdx1.ToString("0.00");


                        // dr2["cloum13"] = Math.Round(gfscl1 / rowcout0);
                        dr2["cloum13"] = (scl1 > 0) ? Math.Round((gfscl1 * 100 / scl1),2):0;
                        dr2["cloum14"] = (devicescount == 0) ? 0 : Math.Round(((double)sbsyl1 * 100 / devicescount),2);//Math.Round(usagerate1, 2);//设备使用率汇总


                            dr2["cloum15"] = sbsyl2;
                        dr2["cloum16"] = sbwsyl2;
                        dr2["cloum17"] = spsc2.ToString("0.00");
                        dr2["cloum18"] = spdx2.ToString("0.00");

                        //  dr2["cloum19"] = Math.Round(gfscl2 / rowcout0);
                        dr2["cloum19"] = (scl2 > 0) ? Math.Round((gfscl2 * 100 / scl2),2):0;
                        dr2["cloum20"] = (devicescount == 0) ? 0 : Math.Round(((double)sbsyl2 * 100 / devicescount), 2);//Math.Round(usagerate2, 2);//设备使用率汇总


                        dr2["cloum21"] = sbsyl3;
                        dr2["cloum22"] = sbwsyl3;
                        dr2["cloum23"] = spsc3.ToString("0.00");
                        dr2["cloum24"] = spdx3.ToString("0.00");



                        // dr2["cloum25"] = Math.Round(gfscl3 / rowcout0);
                        dr2["cloum25"] = (scl3 > 0) ? Math.Round((gfscl3 * 100 / scl3),2):0;
                        dr2["cloum26"] = (devicescount == 0) ? 0 : Math.Round(((double)sbsyl3 * 100 / devicescount), 2);// Math.Round(usagerate3, 2);//设备使用率汇总


                        dr2["cloum27"] = sbsyl4;
                        dr2["cloum28"] = sbwsyl4;
                        dr2["cloum29"] = spsc4.ToString("0.00");
                        dr2["cloum30"] = spdx4.ToString("0.00");


                        // dr2["cloum31"] = Math.Round(gfscl4 / rowcout0);
                        dr2["cloum31"] = (scl4 > 0) ? Math.Round((gfscl4 * 100 / scl4),2):0;
                        dr2["cloum32"] = (devicescount == 0) ? 0 : Math.Round(((double)sbsyl4 * 100 / devicescount), 2);// Math.Round(usagerate4, 2);//设备使用率汇总



                        break;
                    case "1":
                    case "2":
                    case "3":
                    case "7":
                        dr2["cloum3"] = status0;
                        dr2["cloum4"] = zxsc0.ToString("0.00");
                        dr2["cloum5"] = (devicescount == 0) ? 0 : Math.Round(((double)status0 *100 / devicescount),2);//设备使用率汇总

                        dr2["cloum6"] = status1;
                        dr2["cloum7"] = zxsc1.ToString("0.00");
                        dr2["cloum8"] = (devicescount == 0) ? 0 : Math.Round(((double)status1 * 100 / devicescount), 2);//设备使用率汇总

                        dr2["cloum9"] = status2;
                        dr2["cloum10"] = zxsc2.ToString("0.00");
                        dr2["cloum11"] = (devicescount == 0) ? 0 : Math.Round(((double)status2 * 100 / devicescount), 2);//设备使用率汇总

                        dr2["cloum12"] = status3;
                        dr2["cloum13"] = zxsc3.ToString("0.00");
                        dr2["cloum14"] = (devicescount == 0) ? 0 : Math.Round(((double)status3 * 100 / devicescount), 2);//设备使用率汇总

                        dr2["cloum15"] = status4;
                        dr2["cloum16"] = zxsc4.ToString("0.00");
                        dr2["cloum17"] = (devicescount == 0) ? 0 : Math.Round(((double)status4 * 100 / devicescount), 2);//设备使用率汇总


                        break;
                    default:
                        break;
                }
                dtreturns.Rows.Add(dr2);



            }

            #endregion

           string reTitle = ExportExcel(dtreturns, type, begintime, endtime, title, ssdd, sszd, context.Request.Form["ssddtext"], context.Request.Form["sszdtext"]);
           context.Response.Write(JSON.DatatableToDatatableJS(dtreturns, reTitle));
            }
            catch (Exception e)
            {
                context.Response.Write("{\"data\":\"\"}");

            }

        }

        public IEnumerable<entityStruct> GetSonID(string p_id)
        {
            try
            {
                var query = (from p in allEntitys.AsEnumerable()
                             where (p.Field<string>("SJBM") == p_id)
                             select new entityStruct
                             {
                                 BMDM = p.Field<string>("BMDM"),
                                 SJBM = p.Field<string>("SJBM")
                             }).ToList<entityStruct>();
                return query.ToList().Concat(query.ToList().SelectMany(t => GetSonID(t.BMDM)));
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public List<dataStruct> findallchildren(string parentid, DataTable dt)
        {
            var list = (from p in dt.AsEnumerable()
                        where p.Field<string>("ParentID") == parentid
                        select new dataStruct
                        {
                            BMDM = p.Field<string>("BMDM"),
                            ParentID = p.Field<string>("ParentID"),
                            在线时长 = p.Field<int>("在线时长"),
                            文件大小 = p.Field<int>("文件大小"),
                            AlarmType = p.Field<int>("AlarmType"),
                            DevId = p.Field<string>("DevId")
                        }).ToList<dataStruct>();
            if (list.Count != 0)
            {
                tmpList.AddRange(list);
            }
            foreach (dataStruct single in list)
            {
                List<dataStruct> tmpChildren = findallchildren(single.BMDM, dt);

            }
            return tmpList;
        }

        public class entityStruct
        {
            public string BMDM;
            public string SJBM;
        }

        public class dataStruct
        {
            public string BMDM = "BMDM";
            public string ParentID = "ParentID";
            public int 在线时长 = 0;
            public int 文件大小 = 0;
            public int AlarmType = 0;
            public string DevId = "DevId";
            public int UploadCnt = 0;
            public int GFUploadCnt = 0;
            public int HandleCnt = 0;
            public int CXCnt = 0;

        }


        //导出excel 
        public string ExportExcel(DataTable dt, string type, string begintime, string endtime, string entityTitle, string ssdd, string sszd, string ssddtext, string sszdtext)
        {
            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheet sheet = null;
            var tmpath = "";
            string Entityname = "";
            Entityname += (ssddtext == "全部") ? "台州交警局" : ssddtext;
            Entityname += (sszdtext == "全部") ? "" : sszdtext;
            switch (type)
            {
                case "1":
   
                case "3":
                    if (ssdd != "all" && sszd != "all")
                    {
                      
                        tmpath = HttpContext.Current.Server.MapPath("report\\1-3.xls");
                        excelFile.LoadXls(tmpath);
                        sheet = excelFile.Worksheets[0];
                        sheet.Rows[1].Cells["D"].Value = ConfigurationManager.AppSettings["Time1"];
                        sheet.Rows[1].Cells["E"].Value = ConfigurationManager.AppSettings["Time2"];
                        sheet.Rows[1].Cells["F"].Value = ConfigurationManager.AppSettings["Time3"];
                        sheet.Rows[1].Cells["G"].Value = ConfigurationManager.AppSettings["Time4"];
                        sheet.Rows[1].Cells["H"].Value = ConfigurationManager.AppSettings["Time5"];
                    }
                    else
                    {
                        tmpath = HttpContext.Current.Server.MapPath("report\\1.xls");
                        excelFile.LoadXls(tmpath);
                        sheet = excelFile.Worksheets[0];
                        sheet.Rows[1].Cells["C"].Value = ConfigurationManager.AppSettings["Time1"];
                        sheet.Rows[1].Cells["F"].Value = ConfigurationManager.AppSettings["Time2"];
                        sheet.Rows[1].Cells["I"].Value = ConfigurationManager.AppSettings["Time3"];
                        sheet.Rows[1].Cells["L"].Value = ConfigurationManager.AppSettings["Time4"];
                        sheet.Rows[1].Cells["O"].Value = ConfigurationManager.AppSettings["Time5"];
                    }

                    break;

                case "2":
               if (ssdd != "all" && sszd != "all")
                    {

                        tmpath = HttpContext.Current.Server.MapPath("report\\2.xls");
                        excelFile.LoadXls(tmpath);
                        sheet = excelFile.Worksheets[0];
                        sheet.Rows[1].Cells["D"].Value = ConfigurationManager.AppSettings["Time1"];
                        sheet.Rows[1].Cells["E"].Value = ConfigurationManager.AppSettings["Time2"];
                        sheet.Rows[1].Cells["F"].Value = ConfigurationManager.AppSettings["Time3"];
                        sheet.Rows[1].Cells["G"].Value = ConfigurationManager.AppSettings["Time4"];
                        sheet.Rows[1].Cells["H"].Value = ConfigurationManager.AppSettings["Time5"];
                    }
                    else
                    {
                        tmpath = HttpContext.Current.Server.MapPath("report\\1.xls");
                        excelFile.LoadXls(tmpath);
                        sheet = excelFile.Worksheets[0];
                        sheet.Rows[1].Cells["C"].Value = ConfigurationManager.AppSettings["Time1"];
                        sheet.Rows[1].Cells["F"].Value = ConfigurationManager.AppSettings["Time2"];
                        sheet.Rows[1].Cells["I"].Value = ConfigurationManager.AppSettings["Time3"];
                        sheet.Rows[1].Cells["L"].Value = ConfigurationManager.AppSettings["Time4"];
                        sheet.Rows[1].Cells["O"].Value = ConfigurationManager.AppSettings["Time5"];
                    }
                    break;

                case "5":
                    if (ssdd != "all" && sszd != "all")
                    {

                        tmpath = HttpContext.Current.Server.MapPath("report\\5-5.xls");
                        excelFile.LoadXls(tmpath);
                        sheet = excelFile.Worksheets[0];
                        sheet.Rows[1].Cells["D"].Value = ConfigurationManager.AppSettings["Time1"];
                        sheet.Rows[1].Cells["F"].Value = ConfigurationManager.AppSettings["Time2"];
                        sheet.Rows[1].Cells["H"].Value = ConfigurationManager.AppSettings["Time3"];
                        sheet.Rows[1].Cells["J"].Value = ConfigurationManager.AppSettings["Time4"];
                        sheet.Rows[1].Cells["L"].Value = ConfigurationManager.AppSettings["Time5"];
                    }
                    else
                    {
                        tmpath = HttpContext.Current.Server.MapPath("report\\5.xls");
                        excelFile.LoadXls(tmpath);
                        sheet = excelFile.Worksheets[0];
                        sheet.Rows[1].Cells["C"].Value = ConfigurationManager.AppSettings["Time1"];
                        sheet.Rows[1].Cells["I"].Value = ConfigurationManager.AppSettings["Time2"];
                        sheet.Rows[1].Cells["O"].Value = ConfigurationManager.AppSettings["Time3"];
                        sheet.Rows[1].Cells["U"].Value = ConfigurationManager.AppSettings["Time4"];
                        sheet.Rows[1].Cells["AA"].Value = ConfigurationManager.AppSettings["Time5"];
                    }
                    break;
                case "4"://警务通
                    if (ssdd != "all" && sszd != "all")
                    {

                        tmpath = HttpContext.Current.Server.MapPath("report\\4-4.xls");
                        excelFile.LoadXls(tmpath);
                        sheet = excelFile.Worksheets[0];
                        sheet.Rows[1].Cells["D"].Value = ConfigurationManager.AppSettings["Time1"];
                        sheet.Rows[1].Cells["F"].Value = ConfigurationManager.AppSettings["Time2"];
                        sheet.Rows[1].Cells["H"].Value = ConfigurationManager.AppSettings["Time3"];
                        sheet.Rows[1].Cells["J"].Value = ConfigurationManager.AppSettings["Time4"];
                        sheet.Rows[1].Cells["L"].Value = ConfigurationManager.AppSettings["Time5"];
                    }
                    else
                    {
                        tmpath = HttpContext.Current.Server.MapPath("report\\4.xls");
                        excelFile.LoadXls(tmpath);
                        sheet = excelFile.Worksheets[0];
                        sheet.Rows[1].Cells["D"].Value = ConfigurationManager.AppSettings["Time1"];
                        sheet.Rows[1].Cells["I"].Value = ConfigurationManager.AppSettings["Time2"];
                        sheet.Rows[1].Cells["N"].Value = ConfigurationManager.AppSettings["Time3"];
                        sheet.Rows[1].Cells["S"].Value = ConfigurationManager.AppSettings["Time4"];
                        sheet.Rows[1].Cells["X"].Value = ConfigurationManager.AppSettings["Time5"];
                    }
                    break;
                case "6"://辅警通
                    if (ssdd != "all" && sszd != "all")
                    {

                        tmpath = HttpContext.Current.Server.MapPath("report\\6-6.xls");
                        excelFile.LoadXls(tmpath);
                        sheet = excelFile.Worksheets[0];
                        sheet.Rows[1].Cells["D"].Value = ConfigurationManager.AppSettings["Time1"];
                        sheet.Rows[1].Cells["F"].Value = ConfigurationManager.AppSettings["Time2"];
                        sheet.Rows[1].Cells["H"].Value = ConfigurationManager.AppSettings["Time3"];
                        sheet.Rows[1].Cells["J"].Value = ConfigurationManager.AppSettings["Time4"];
                        sheet.Rows[1].Cells["L"].Value = ConfigurationManager.AppSettings["Time5"];
                    }
                    else
                    {
                        tmpath = HttpContext.Current.Server.MapPath("report\\6.xls");
                        excelFile.LoadXls(tmpath);
                        sheet = excelFile.Worksheets[0];
                        sheet.Rows[1].Cells["D"].Value = ConfigurationManager.AppSettings["Time1"];
                        sheet.Rows[1].Cells["I"].Value = ConfigurationManager.AppSettings["Time2"];
                        sheet.Rows[1].Cells["N"].Value = ConfigurationManager.AppSettings["Time3"];
                        sheet.Rows[1].Cells["S"].Value = ConfigurationManager.AppSettings["Time4"];
                        sheet.Rows[1].Cells["X"].Value = ConfigurationManager.AppSettings["Time5"];
                    }
                    break;

            }

           


            DateTime bg = Convert.ToDateTime(begintime);
            DateTime ed = Convert.ToDateTime(endtime);
            string typename = "";
            switch (type)
            {
                case "1":
                    typename = "车载视频";
                    break;
                case "2":
                    typename = "对讲机";
                    break;
                case "3":
                    typename = "拦截仪";
                    break;
                case "5":
                    typename = "执法记录仪";
                    break;
                case "4":
                    typename = "警务通";
                    break;
                case "6":
                    typename = "辅警通";
                    break;
            }


            sheet.Rows[0].Cells["A"].Value = begintime.Replace("/", "-") + "_" + endtime.Replace("/", "-") + Entityname + typename + "报表";
            switch (type)
            {
                case "1":
                case "2":
                case "3":
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        if (ssdd != "all" && sszd != "all")
                        {
                            sheet.Rows[i + 3].Cells["A"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["A"].Value = dt.Rows[i][0].ToString();
                            sheet.Rows[i + 3].Cells["B"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["B"].Value = dt.Rows[i][1].ToString();
                            sheet.Rows[i + 3].Cells["C"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["C"].Value = dt.Rows[i][2].ToString();
                            sheet.Rows[i + 3].Cells["D"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["D"].Value = dt.Rows[i][3].ToString();
                            sheet.Rows[i + 3].Cells["E"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["E"].Value = dt.Rows[i][4].ToString();
                            sheet.Rows[i + 3].Cells["F"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["F"].Value = dt.Rows[i][5].ToString();
                            sheet.Rows[i + 3].Cells["G"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["G"].Value = dt.Rows[i][6].ToString();
                            sheet.Rows[i + 3].Cells["H"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["H"].Value = dt.Rows[i][7].ToString();

                        }
                        else
                        {

                            sheet.Rows[i + 3].Cells["A"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["A"].Value = dt.Rows[i][0].ToString();
                            sheet.Rows[i + 3].Cells["B"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["B"].Value = dt.Rows[i][1].ToString();
                            sheet.Rows[i + 3].Cells["C"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["C"].Value = dt.Rows[i][2].ToString();
                            sheet.Rows[i + 3].Cells["D"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["D"].Value = dt.Rows[i][3].ToString();
                            sheet.Rows[i + 3].Cells["E"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["E"].Value = dt.Rows[i][4].ToString();
                            sheet.Rows[i + 3].Cells["F"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["F"].Value = dt.Rows[i][5].ToString();
                            sheet.Rows[i + 3].Cells["G"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["G"].Value = dt.Rows[i][6].ToString();


                            sheet.Rows[i + 3].Cells["H"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["H"].Value = dt.Rows[i][7].ToString();
                            sheet.Rows[i + 3].Cells["I"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["I"].Value = dt.Rows[i][8].ToString();
                            sheet.Rows[i + 3].Cells["J"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["J"].Value = dt.Rows[i][9].ToString();
                            sheet.Rows[i + 3].Cells["K"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["K"].Value = dt.Rows[i][10].ToString();
                            sheet.Rows[i + 3].Cells["L"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["L"].Value = dt.Rows[i][11].ToString();
                            sheet.Rows[i + 3].Cells["M"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["M"].Value = dt.Rows[i][12].ToString();
                            sheet.Rows[i + 3].Cells["N"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["N"].Value = dt.Rows[i][13].ToString();


                            sheet.Rows[i + 3].Cells["O"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["O"].Value = dt.Rows[i][14].ToString();
                            sheet.Rows[i + 3].Cells["P"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["P"].Value = dt.Rows[i][15].ToString();
                            sheet.Rows[i + 3].Cells["Q"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["Q"].Value = dt.Rows[i][16].ToString();

                        }


                    }
                    break;

                case "5":
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {


                        if (ssdd != "all" && sszd != "all")
                        {
                            sheet.Rows[i + 3].Cells["A"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["A"].Value = dt.Rows[i][0].ToString();
                            sheet.Rows[i + 3].Cells["B"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["B"].Value = dt.Rows[i][1].ToString();
                            sheet.Rows[i + 3].Cells["C"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["C"].Value = dt.Rows[i][2].ToString();
                            sheet.Rows[i + 3].Cells["D"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["D"].Value = dt.Rows[i][3].ToString();
                            sheet.Rows[i + 3].Cells["E"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["E"].Value = dt.Rows[i][4].ToString();
                            sheet.Rows[i + 3].Cells["F"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["F"].Value = dt.Rows[i][5].ToString();
                            sheet.Rows[i + 3].Cells["G"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["G"].Value = dt.Rows[i][6].ToString();
                            sheet.Rows[i + 3].Cells["H"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["H"].Value = dt.Rows[i][7].ToString();
                            sheet.Rows[i + 3].Cells["I"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["I"].Value = dt.Rows[i][8].ToString();
                            sheet.Rows[i + 3].Cells["J"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["J"].Value = dt.Rows[i][9].ToString();
                            sheet.Rows[i + 3].Cells["K"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["K"].Value = dt.Rows[i][10].ToString();
                            sheet.Rows[i + 3].Cells["L"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["L"].Value = dt.Rows[i][11].ToString();
                            sheet.Rows[i + 3].Cells["M"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["M"].Value = dt.Rows[i][12].ToString();


                        }

                        else
                        {
                            sheet.Rows[i + 3].Cells["A"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["A"].Value = dt.Rows[i][0].ToString();
                            sheet.Rows[i + 3].Cells["B"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["B"].Value = dt.Rows[i][1].ToString();
                            sheet.Rows[i + 3].Cells["C"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["C"].Value = dt.Rows[i][2].ToString();
                            sheet.Rows[i + 3].Cells["D"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["D"].Value = dt.Rows[i][3].ToString();
                            sheet.Rows[i + 3].Cells["E"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["E"].Value = dt.Rows[i][4].ToString();
                            sheet.Rows[i + 3].Cells["F"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["F"].Value = dt.Rows[i][5].ToString();
                            sheet.Rows[i + 3].Cells["G"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["G"].Value = dt.Rows[i][6].ToString();
                            sheet.Rows[i + 3].Cells["H"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["H"].Value = dt.Rows[i][7].ToString();
                            sheet.Rows[i + 3].Cells["I"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["I"].Value = dt.Rows[i][8].ToString();
                            sheet.Rows[i + 3].Cells["J"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["J"].Value = dt.Rows[i][9].ToString();



                            sheet.Rows[i + 3].Cells["K"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["K"].Value = dt.Rows[i][10].ToString();
                            sheet.Rows[i + 3].Cells["L"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["L"].Value = dt.Rows[i][11].ToString();
                            sheet.Rows[i + 3].Cells["M"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["M"].Value = dt.Rows[i][12].ToString();
                            sheet.Rows[i + 3].Cells["N"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["N"].Value = dt.Rows[i][13].ToString();
                            sheet.Rows[i + 3].Cells["O"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["O"].Value = dt.Rows[i][14].ToString();
                            sheet.Rows[i + 3].Cells["P"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["P"].Value = dt.Rows[i][15].ToString();
                            sheet.Rows[i + 3].Cells["Q"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["Q"].Value = dt.Rows[i][16].ToString();
                            sheet.Rows[i + 3].Cells["R"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["R"].Value = dt.Rows[i][17].ToString();
                            sheet.Rows[i + 3].Cells["S"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["S"].Value = dt.Rows[i][18].ToString();
                            sheet.Rows[i + 3].Cells["T"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["T"].Value = dt.Rows[i][19].ToString();



                            sheet.Rows[i + 3].Cells["U"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["U"].Value = dt.Rows[i][20].ToString();
                            sheet.Rows[i + 3].Cells["V"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["V"].Value = dt.Rows[i][21].ToString();
                            sheet.Rows[i + 3].Cells["W"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["W"].Value = dt.Rows[i][22].ToString();
                            sheet.Rows[i + 3].Cells["X"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["X"].Value = dt.Rows[i][23].ToString();
                            sheet.Rows[i + 3].Cells["Y"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["Y"].Value = dt.Rows[i][24].ToString();
                            sheet.Rows[i + 3].Cells["Z"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["Z"].Value = dt.Rows[i][25].ToString();

                            sheet.Rows[i + 3].Cells[26].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells[26].Value = dt.Rows[i][26].ToString();
                            sheet.Rows[i + 3].Cells[27].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells[27].Value = dt.Rows[i][27].ToString();
                            sheet.Rows[i + 3].Cells[28].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells[28].Value = dt.Rows[i][28].ToString();


                            sheet.Rows[i + 3].Cells[29].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells[29].Value = dt.Rows[i][29].ToString();
                            sheet.Rows[i + 3].Cells[30].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells[30].Value = dt.Rows[i][30].ToString();
                            sheet.Rows[i + 3].Cells[31].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells[31].Value = dt.Rows[i][31].ToString();
                        }
   
                    }

                    break;
                case "4":
                case "6":
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        if (ssdd != "all" && sszd != "all")
                        {
                            sheet.Rows[i + 3].Cells["A"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["A"].Value = dt.Rows[i][0].ToString();
                            sheet.Rows[i + 3].Cells["B"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["B"].Value = dt.Rows[i][1].ToString();
                            sheet.Rows[i + 3].Cells["C"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["C"].Value = dt.Rows[i][2].ToString();
                            sheet.Rows[i + 3].Cells["D"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["D"].Value = dt.Rows[i][3].ToString();
                            sheet.Rows[i + 3].Cells["E"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["E"].Value = dt.Rows[i][4].ToString();
                            sheet.Rows[i + 3].Cells["F"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["F"].Value = dt.Rows[i][5].ToString();
                            sheet.Rows[i + 3].Cells["G"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["G"].Value = dt.Rows[i][6].ToString();
                            sheet.Rows[i + 3].Cells["H"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["H"].Value = dt.Rows[i][7].ToString();
                            sheet.Rows[i + 3].Cells["I"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["I"].Value = dt.Rows[i][8].ToString();
                            sheet.Rows[i + 3].Cells["J"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["J"].Value = dt.Rows[i][9].ToString();
                            sheet.Rows[i + 3].Cells["K"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["K"].Value = dt.Rows[i][10].ToString();
                            sheet.Rows[i + 3].Cells["L"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["L"].Value = dt.Rows[i][11].ToString();
                            sheet.Rows[i + 3].Cells["M"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["M"].Value = dt.Rows[i][12].ToString();
        

                        }


                        else
                        {
                            sheet.Rows[i + 3].Cells["A"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["A"].Value = dt.Rows[i][0].ToString();
                            sheet.Rows[i + 3].Cells["B"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["B"].Value = dt.Rows[i][1].ToString();
                            sheet.Rows[i + 3].Cells["C"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["C"].Value = dt.Rows[i][2].ToString();
                            sheet.Rows[i + 3].Cells["D"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["D"].Value = dt.Rows[i][3].ToString();
                            sheet.Rows[i + 3].Cells["E"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["E"].Value = dt.Rows[i][4].ToString();
                            sheet.Rows[i + 3].Cells["F"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["F"].Value = dt.Rows[i][5].ToString();
                            sheet.Rows[i + 3].Cells["G"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["G"].Value = dt.Rows[i][6].ToString();
                            sheet.Rows[i + 3].Cells["H"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["H"].Value = dt.Rows[i][7].ToString();
                            sheet.Rows[i + 3].Cells["I"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["I"].Value = dt.Rows[i][8].ToString();
                            sheet.Rows[i + 3].Cells["J"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["J"].Value = dt.Rows[i][9].ToString();



                            sheet.Rows[i + 3].Cells["K"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["K"].Value = dt.Rows[i][10].ToString();
                            sheet.Rows[i + 3].Cells["L"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["L"].Value = dt.Rows[i][11].ToString();
                            sheet.Rows[i + 3].Cells["M"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["M"].Value = dt.Rows[i][12].ToString();
                            sheet.Rows[i + 3].Cells["N"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["N"].Value = dt.Rows[i][13].ToString();
                            sheet.Rows[i + 3].Cells["O"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["O"].Value = dt.Rows[i][14].ToString();
                            sheet.Rows[i + 3].Cells["P"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["P"].Value = dt.Rows[i][15].ToString();
                            sheet.Rows[i + 3].Cells["Q"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["Q"].Value = dt.Rows[i][16].ToString();
                            sheet.Rows[i + 3].Cells["R"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["R"].Value = dt.Rows[i][17].ToString();
                            sheet.Rows[i + 3].Cells["S"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["S"].Value = dt.Rows[i][18].ToString();
                            sheet.Rows[i + 3].Cells["T"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["T"].Value = dt.Rows[i][19].ToString();



                            sheet.Rows[i + 3].Cells["U"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["U"].Value = dt.Rows[i][20].ToString();
                            sheet.Rows[i + 3].Cells["V"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["V"].Value = dt.Rows[i][21].ToString();
                            sheet.Rows[i + 3].Cells["W"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["W"].Value = dt.Rows[i][22].ToString();
                            sheet.Rows[i + 3].Cells["X"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["X"].Value = dt.Rows[i][23].ToString();
                            sheet.Rows[i + 3].Cells["Y"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["Y"].Value = dt.Rows[i][24].ToString();
                            sheet.Rows[i + 3].Cells["Z"].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells["Z"].Value = dt.Rows[i][25].ToString();

                            sheet.Rows[i + 3].Cells[26].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells[26].Value = dt.Rows[i][26].ToString();
                            sheet.Rows[i + 3].Cells[27].Style.Borders.SetBorders(MultipleBorders.Outside, Color.FromArgb(0, 0, 0), LineStyle.Thin);
                            sheet.Rows[i + 3].Cells[27].Value = dt.Rows[i][27].ToString();

                        }

                    }

                    break;

            }


            tmpath = HttpContext.Current.Server.MapPath("upload\\" + sheet.Rows[0].Cells[0].Value + ".xls");

            excelFile.SaveXls(tmpath);
            return sheet.Rows[0].Cells[0].Value + ".xls";
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