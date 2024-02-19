using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamUp.Areas.Panel.Pages
{
    public static class PanelNavPages
    {
        public static string Index => "Index";

        public static string Applications => "Applications";

        public static string Calendar => "Calendar";

        public static string Chat => "Chat";

        public static string Post => "Post";

        public static string Settings => "Settings";

        public static string Tasks => "Tasks";

        public static string Users => "Users";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string ApplicationsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Applications);

        public static string CalendarNavClass(ViewContext viewContext) => PageNavClass(viewContext, Calendar);

        public static string ChatNavClass(ViewContext viewContext) => PageNavClass(viewContext, Chat);

        public static string PostNavClass(ViewContext viewContext) => PageNavClass(viewContext, Post);

        public static string SettingsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Settings);

        public static string TasksNavClass(ViewContext viewContext) => PageNavClass(viewContext, Tasks);

        public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
