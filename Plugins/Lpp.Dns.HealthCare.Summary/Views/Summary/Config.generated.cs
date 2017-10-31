﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lpp.Dns.HealthCare.Summary.Views.Summary
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using Lpp;
    
    #line 1 "..\..\Views\Summary\Config.cshtml"
    using Lpp.Dns;
    
    #line default
    #line hidden
    using Lpp.Dns.HealthCare.Summary.Models;
    using Lpp.Mvc;
    using Lpp.Mvc.Application;
    using Lpp.Mvc.Controls;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Summary/Config.cshtml")]
    public partial class Config : System.Web.Mvc.WebViewPage<ConfigModel>
    {
        public Config()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\Summary\Config.cshtml"
  
    Layout = null;
    var id = Html.UniqueId();

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n<div");

WriteLiteral(" class=\"Value ModelConfigForm\"");

WriteAttribute("id", Tuple.Create(" id=\"", 130), Tuple.Create("\"", 138)
            
            #line 8 "..\..\Views\Summary\Config.cshtml"
, Tuple.Create(Tuple.Create("", 135), Tuple.Create<System.Object, System.Int32>(id
            
            #line default
            #line hidden
, 135), false)
);

WriteLiteral(">\r\n    <a");

WriteLiteral(" href=\"#\"");

WriteLiteral(" class=\"Link\"");

WriteLiteral(">[configure]</a>\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        $(function () {\r\n            var dlg = $(\"#");

            
            #line 12 "..\..\Views\Summary\Config.cshtml"
                      Write(id);

            
            #line default
            #line hidden
WriteLiteral(" > .ModelConfig\");\r\n            var link = $(\"#");

            
            #line 13 "..\..\Views\Summary\Config.cshtml"
                       Write(id);

            
            #line default
            #line hidden
WriteLiteral(" > a.Link\");\r\n            link.click(function () {\r\n                dlg.dialog({\r" +
"\n                    modal: true, title: \"");

            
            #line 16 "..\..\Views\Summary\Config.cshtml"
                                    Write(Model.Model.Name);

            
            #line default
            #line hidden
WriteLiteral(@""",
                    width: 540, buttons: {
                        OK: function () {
                            dlg.dialog(""close"");
                        },
                        Cancel: function () { dlg.dialog(""close""); }
                    }
                });
                return false;
            });
            var initialDataProvider = $(""#");

            
            #line 26 "..\..\Views\Summary\Config.cshtml"
                                      Write(Model.Model.ID);

            
            #line default
            #line hidden
WriteLiteral("_DataProvider\").val();\r\n            showParameters_");

            
            #line 27 "..\..\Views\Summary\Config.cshtml"
                       Write(Model.Model.ID.ToString("N"));

            
            #line default
            #line hidden
WriteLiteral("(initialDataProvider);\r\n        });\r\n\r\n        function showParameters_");

            
            #line 30 "..\..\Views\Summary\Config.cshtml"
                            Write(Model.Model.ID.ToString("N"));

            
            #line default
            #line hidden
WriteLiteral("(value) {\r\n            $(\"#");

            
            #line 31 "..\..\Views\Summary\Config.cshtml"
            Write(Model.Model.ID);

            
            #line default
            #line hidden
WriteLiteral("_DataProvider\").val(value);\r\n            switch (value) {\r\n                case \"" +
"ODBC\":\r\n                    $(\"#");

            
            #line 34 "..\..\Views\Summary\Config.cshtml"
                    Write(Model.Model.ID);

            
            #line default
            #line hidden
WriteLiteral("_divODBC\").show();\r\n                    $(\"#");

            
            #line 35 "..\..\Views\Summary\Config.cshtml"
                    Write(Model.Model.ID);

            
            #line default
            #line hidden
WriteLiteral("_divSQLServer\").hide();\r\n                    break;\r\n\r\n                case \"SQLS" +
"erver\":\r\n                    $(\"#");

            
            #line 39 "..\..\Views\Summary\Config.cshtml"
                    Write(Model.Model.ID);

            
            #line default
            #line hidden
WriteLiteral("_divODBC\").hide();\r\n                    $(\"#");

            
            #line 40 "..\..\Views\Summary\Config.cshtml"
                    Write(Model.Model.ID);

            
            #line default
            #line hidden
WriteLiteral("_divSQLServer\").show();\r\n                    break;\r\n            }\r\n        }\r\n  " +
"  </script>\r\n\r\n    <div");

WriteAttribute("id", Tuple.Create(" id=\"", 1578), Tuple.Create("\"", 1598)
            
            #line 46 "..\..\Views\Summary\Config.cshtml"
, Tuple.Create(Tuple.Create("", 1583), Tuple.Create<System.Object, System.Int32>(Model.Model.ID
            
            #line default
            #line hidden
, 1583), false)
);

WriteLiteral(" class=\"ModelConfig\"");

WriteLiteral(" style=\"display: none;\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"ui-groupbox\"");

WriteLiteral(" style=\"height: 305px;\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"ui-dialog-groupbox-header\"");

WriteLiteral("><span>Data Source</span></div>\r\n            <div");

WriteLiteral(" class=\"ui-form\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"Field\"");

WriteLiteral(">\r\n                    <label>Data Provider</label>\r\n                    <select");

WriteAttribute("id", Tuple.Create(" id=\"", 1935), Tuple.Create("\"", 1976)
            
            #line 52 "..\..\Views\Summary\Config.cshtml"
, Tuple.Create(Tuple.Create("", 1940), Tuple.Create<System.Object, System.Int32>(Model.Model.ID
            
            #line default
            #line hidden
, 1940), false)
, Tuple.Create(Tuple.Create("", 1957), Tuple.Create("_DataProviderSelect", 1957), true)
);

WriteLiteral(" name=\"DataProviderSelect\"");

WriteAttribute("onchange", Tuple.Create(" onchange=\"", 2003), Tuple.Create("\"", 2076)
, Tuple.Create(Tuple.Create("", 2014), Tuple.Create("showParameters_", 2014), true)
            
            #line 52 "..\..\Views\Summary\Config.cshtml"
                                          , Tuple.Create(Tuple.Create("", 2029), Tuple.Create<System.Object, System.Int32>(Model.Model.ID.ToString("N")
            
            #line default
            #line hidden
, 2029), false)
, Tuple.Create(Tuple.Create("", 2060), Tuple.Create("($(this).val());", 2060), true)
);

WriteLiteral(">\r\n                        <option");

WriteLiteral(" value=\"SQLServer\"");

WriteLiteral(" ");

            
            #line 53 "..\..\Views\Summary\Config.cshtml"
                                              Write(Model.Properties.ContainsKey("DataProvider") && Model.Properties["DataProvider"] == "SQLServer" ? "selected=\"selected\"" : "");

            
            #line default
            #line hidden
WriteLiteral(">SQL Server</option>\r\n                        <option");

WriteLiteral(" value=\"ODBC\"");

WriteLiteral(" ");

            
            #line 54 "..\..\Views\Summary\Config.cshtml"
                                         Write(Model.Properties.ContainsKey("DataProvider") && Model.Properties["DataProvider"] == "ODBC" ? "selected=\"selected\"" : "");

            
            #line default
            #line hidden
WriteLiteral(">ODBC</option>\r\n                    </select>\r\n                    <input");

WriteAttribute("id", Tuple.Create(" id=\"", 2523), Tuple.Create("\"", 2558)
            
            #line 56 "..\..\Views\Summary\Config.cshtml"
, Tuple.Create(Tuple.Create("", 2528), Tuple.Create<System.Object, System.Int32>(Model.Model.ID
            
            #line default
            #line hidden
, 2528), false)
, Tuple.Create(Tuple.Create("", 2545), Tuple.Create("_DataProvider", 2545), true)
);

WriteLiteral(" name=\"DataProvider\"");

WriteLiteral(" type=\"hidden\"");

WriteAttribute("value", Tuple.Create(" value=\"", 2593), Tuple.Create("\"", 2697)
            
            #line 56 "..\..\Views\Summary\Config.cshtml"
                         , Tuple.Create(Tuple.Create("", 2601), Tuple.Create<System.Object, System.Int32>(Model.Properties.ContainsKey("DataProvider") ? Model.Properties["DataProvider"] : "SQLServer"
            
            #line default
            #line hidden
, 2601), false)
);

WriteLiteral(" />\r\n                </div>\r\n                <br />\r\n            </div>\r\n        " +
"    <div");

WriteAttribute("id", Tuple.Create(" id=\"", 2787), Tuple.Create("\"", 2817)
            
            #line 60 "..\..\Views\Summary\Config.cshtml"
, Tuple.Create(Tuple.Create("", 2792), Tuple.Create<System.Object, System.Int32>(Model.Model.ID
            
            #line default
            #line hidden
, 2792), false)
, Tuple.Create(Tuple.Create("", 2809), Tuple.Create("_divODBC", 2809), true)
);

WriteLiteral(" class=\"ui-form\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"Field\"");

WriteLiteral(">\r\n                    <label>DataMart ODBC Data Source Name</label>\r\n           " +
"         <input");

WriteLiteral(" id=\"DataSourceName\"");

WriteLiteral(" name=\"DataSourceName\"");

WriteLiteral(" type=\"text\"");

WriteAttribute("value", Tuple.Create(" value=\"", 3021), Tuple.Create("\"", 3120)
            
            #line 63 "..\..\Views\Summary\Config.cshtml"
         , Tuple.Create(Tuple.Create("", 3029), Tuple.Create<System.Object, System.Int32>(Model.Properties.ContainsKey("DataSourceName") ? Model.Properties["DataSourceName"] : ""
            
            #line default
            #line hidden
, 3029), false)
);

WriteLiteral(" />\r\n                </div>\r\n            </div>\r\n            <div");

WriteAttribute("id", Tuple.Create(" id=\"", 3186), Tuple.Create("\"", 3221)
            
            #line 66 "..\..\Views\Summary\Config.cshtml"
, Tuple.Create(Tuple.Create("", 3191), Tuple.Create<System.Object, System.Int32>(Model.Model.ID
            
            #line default
            #line hidden
, 3191), false)
, Tuple.Create(Tuple.Create("", 3208), Tuple.Create("_divSQLServer", 3208), true)
);

WriteLiteral(" class=\"ui-form\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"Field\"");

WriteLiteral(">\r\n                    <label>Server</label>\r\n                    <input");

WriteLiteral(" id=\"Server\"");

WriteLiteral(" name=\"Server\"");

WriteLiteral(" type=\"text\"");

WriteAttribute("value", Tuple.Create(" value=\"", 3385), Tuple.Create("\"", 3468)
            
            #line 69 "..\..\Views\Summary\Config.cshtml"
, Tuple.Create(Tuple.Create("", 3393), Tuple.Create<System.Object, System.Int32>(Model.Properties.ContainsKey("Server") ? Model.Properties["Server"] : ""
            
            #line default
            #line hidden
, 3393), false)
);

WriteLiteral(" />\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"Field\"");

WriteLiteral(">\r\n                    <label>Port</label>\r\n                    <input");

WriteLiteral(" id=\"Port\"");

WriteLiteral(" name=\"Port\"");

WriteLiteral(" type=\"text\"");

WriteAttribute("value", Tuple.Create(" value=\"", 3636), Tuple.Create("\"", 3715)
            
            #line 73 "..\..\Views\Summary\Config.cshtml"
, Tuple.Create(Tuple.Create("", 3644), Tuple.Create<System.Object, System.Int32>(Model.Properties.ContainsKey("Port") ? Model.Properties["Port"] : ""
            
            #line default
            #line hidden
, 3644), false)
);

WriteLiteral(" />\r\n                </div>\r\n\r\n                <div");

WriteLiteral(" class=\"Field\"");

WriteLiteral(">\r\n                    <label>User ID</label>\r\n                    <input");

WriteLiteral(" id=\"UserId\"");

WriteLiteral(" name=\"UserId\"");

WriteLiteral(" type=\"text\"");

WriteAttribute("value", Tuple.Create(" value=\"", 3892), Tuple.Create("\"", 3975)
            
            #line 78 "..\..\Views\Summary\Config.cshtml"
, Tuple.Create(Tuple.Create("", 3900), Tuple.Create<System.Object, System.Int32>(Model.Properties.ContainsKey("UserId") ? Model.Properties["UserId"] : ""
            
            #line default
            #line hidden
, 3900), false)
);

WriteLiteral(" />\r\n                </div>\r\n                <br />\r\n\r\n                <div");

WriteLiteral(" class=\"Field\"");

WriteLiteral(">\r\n                    <label>Password</label>\r\n                    <input");

WriteLiteral(" id=\"Password\"");

WriteLiteral(" name=\"Password\"");

WriteLiteral(" type=\"password\"");

WriteAttribute("value", Tuple.Create(" value=\"", 4185), Tuple.Create("\"", 4272)
            
            #line 84 "..\..\Views\Summary\Config.cshtml"
 , Tuple.Create(Tuple.Create("", 4193), Tuple.Create<System.Object, System.Int32>(Model.Properties.ContainsKey("Password") ? Model.Properties["Password"] : ""
            
            #line default
            #line hidden
, 4193), false)
);

WriteLiteral(" />\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"Field\"");

WriteLiteral(">\r\n                    <label>Confirm Password</label>\r\n                    <inpu" +
"t");

WriteLiteral(" id=\"ConfirmPassword\"");

WriteLiteral(" name=\"ConfirmPassword\"");

WriteLiteral(" type=\"password\"");

WriteAttribute("value", Tuple.Create(" value=\"", 4478), Tuple.Create("\"", 4579)
            
            #line 88 "..\..\Views\Summary\Config.cshtml"
               , Tuple.Create(Tuple.Create("", 4486), Tuple.Create<System.Object, System.Int32>(Model.Properties.ContainsKey("ConfirmPassword") ? Model.Properties["ConfirmPassword"] : ""
            
            #line default
            #line hidden
, 4486), false)
);

WriteLiteral(" />\r\n                </div>\r\n\r\n                <div");

WriteLiteral(" class=\"Field\"");

WriteLiteral(">\r\n                    <label>Database</label>\r\n                    <input");

WriteLiteral(" id=\"Database\"");

WriteLiteral(" name=\"Database\"");

WriteLiteral(" type=\"text\"");

WriteAttribute("value", Tuple.Create(" value=\"", 4761), Tuple.Create("\"", 4848)
            
            #line 93 "..\..\Views\Summary\Config.cshtml"
, Tuple.Create(Tuple.Create("", 4769), Tuple.Create<System.Object, System.Int32>(Model.Properties.ContainsKey("Database") ? Model.Properties["Database"] : ""
            
            #line default
            #line hidden
, 4769), false)
);

WriteLiteral(" />\r\n                </div>\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"ui-form\"");

WriteLiteral(">\r\n                <br />\r\n\r\n                <div");

WriteLiteral(" class=\"Field\"");

WriteLiteral(">\r\n                    <label>Connection Timeout</label>\r\n                    <in" +
"put");

WriteLiteral(" id=\"ConnectionTimeout\"");

WriteLiteral(" name=\"ConnectionTimeout\"");

WriteLiteral(" type=\"text\"");

WriteAttribute("value", Tuple.Create(" value=\"", 5137), Tuple.Create("\"", 5244)
            
            #line 101 "..\..\Views\Summary\Config.cshtml"
               , Tuple.Create(Tuple.Create("", 5145), Tuple.Create<System.Object, System.Int32>(Model.Properties.ContainsKey("ConnectionTimeout") ? Model.Properties["ConnectionTimeout"] : "15"
            
            #line default
            #line hidden
, 5145), false)
);

WriteLiteral(" />\r\n                </div>\r\n\r\n                <div");

WriteLiteral(" class=\"Field\"");

WriteLiteral(">\r\n                    <label>Command Timeout</label>\r\n                    <input" +
"");

WriteLiteral(" id=\"CommandTimeout\"");

WriteLiteral(" name=\"CommandTimeout\"");

WriteLiteral(" type=\"text\"");

WriteAttribute("value", Tuple.Create(" value=\"", 5445), Tuple.Create("\"", 5547)
            
            #line 106 "..\..\Views\Summary\Config.cshtml"
         , Tuple.Create(Tuple.Create("", 5453), Tuple.Create<System.Object, System.Int32>(Model.Properties.ContainsKey("CommandTimeout") ? Model.Properties["CommandTimeout"] : "120"
            
            #line default
            #line hidden
, 5453), false)
);

WriteLiteral(" />\r\n                </div>\r\n            </div>\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"ui-groupbox\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"ui-dialog-groupbox-header\"");

WriteLiteral("><span>Low Cell Counts</span></div>\r\n            <div");

WriteLiteral(" class=\"ui-form\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"Field\"");

WriteLiteral(">\r\n                    <label>Set counts that fall below this number to 0 (option" +
"al)</label>\r\n                    <input");

WriteLiteral(" id=\"ThreshHoldCellCount\"");

WriteLiteral(" name=\"ThreshHoldCellCount\"");

WriteLiteral(" type=\"text\"");

WriteAttribute("value", Tuple.Create(" value=\"", 5990), Tuple.Create("\"", 6099)
            
            #line 116 "..\..\Views\Summary\Config.cshtml"
                   , Tuple.Create(Tuple.Create("", 5998), Tuple.Create<System.Object, System.Int32>(Model.Properties.ContainsKey("ThreshHoldCellCount") ? Model.Properties["ThreshHoldCellCount"] : ""
            
            #line default
            #line hidden
, 5998), false)
);

WriteLiteral(" />\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</di" +
"v>\r\n");

        }
    }
}
#pragma warning restore 1591