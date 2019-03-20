using DbComponent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace JingWuTong.Handle
{
    /// <summary>
    /// permissions_load 的摘要说明
    /// </summary>
    public class permissions_load : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string page_name = context.Request.Form["page_name"];  //page权限或按钮权限
            string type = context.Request.Form["type"];  //page权限或按钮权限
            DataTable rolo_power = null;
            var jybh = "0";
            if (HttpContext.Current.Request.Cookies["cookieName"] != null)
            {

                HttpCookie cookies = HttpContext.Current.Request.Cookies["cookieName"];

                if (cookies["JYBH"] != null)
                {
                    jybh = HttpContext.Current.Server.UrlDecode(cookies["JYBH"]);

                }


            }
            switch (type)
            {
                case "button":
                    rolo_power = SQLHelper.ExecuteRead(CommandType.Text, "SELECT bt.name,enable,type FROM [role_power] rp INNER JOIN Buttons bt on bt.id=rp.page_or_buttons_id WHERE type='button' and page_name ='" + page_name + "' and role_id =(SELECT JSID from ACL_USER where jybh='" + jybh + "') ", "rolo_power");
                    break;
                case "page":
                    rolo_power = SQLHelper.ExecuteRead(CommandType.Text, "SELECT bt.name,enable,type FROM [role_power] rp INNER JOIN Pages bt on bt.id=rp.page_or_buttons_id WHERE type='page' and enable=0 and role_id =(SELECT JSID from ACL_USER where jybh='" + jybh + "')", "rolo_power");
                    break;
                default:
                    rolo_power = SQLHelper.ExecuteRead(CommandType.Text, "SELECT bt.name,enable,type FROM [role_power] rp INNER JOIN Buttons bt on bt.id=rp.page_or_buttons_id WHERE type='button' and page_name ='" + page_name + "' and role_id =(SELECT JSID from ACL_USER where jybh='" + jybh + "') union all SELECT bt.name,enable,type FROM [role_power] rp INNER JOIN Pages bt on bt.id=rp.page_or_buttons_id WHERE type='page' and enable=0 and role_id =(SELECT JSID from ACL_USER where jybh='" + jybh + "')", "rolo_power");
                    break;
            }
            context.Response.Write(JSON.DatatableToDatatableJS(rolo_power, ""));


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