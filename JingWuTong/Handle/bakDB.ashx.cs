using DbComponent;
using GemBox.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using WSExportExcell;

namespace JingWuTong.Handle
{
    /// <summary>
    /// bakDB 的摘要说明
    /// </summary>
    public class bakDB : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var tmpath = "  ";
            var filename = "bakDB_" + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "");
            tmpath = HttpContext.Current.Server.MapPath("upload\\" + filename + ".xls");
            string[] dbs = new string[] { "ACL_USER", "Device", "Entity", "Gps", "Position", "Role", "ProjectDetail", "ProjectFZR", "ProjectInfo", "Buttons", "Pages", "role_power" };
            Excell.WSExportExcell(tmpath, dbs);
            StringBuilder retJson = new StringBuilder();
            retJson.Append("{\"");
            retJson.Append("data");
            retJson.Append('"');
            retJson.Append(":");
            retJson.Append('"');
            retJson.Append(filename);
            retJson.Append('"');
            retJson.Append("}");
            context.Response.Write(retJson);
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