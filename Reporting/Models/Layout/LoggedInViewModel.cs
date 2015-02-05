using System.Collections.Generic;
using dotless.Core.Utils;

namespace InsideReporting.Models.Layout
{
    public class LoggedInViewModel
    {
        public LoggedInViewModel()
        {
            
            TopMenu = new List<MenuItem>();
            SideBarMenu = new List<SideBarMenuItem>();

            if (Roles == null) return;
            if (Roles.Contains("consultant") && !Roles.Contains("sales"))
            {
                AddAdminMenu();
            }
            if (Roles.Contains("sales"))
            {
                AddSalesMenu();
            }
        }

        public void AddMenu()
        {
            if (Roles.Contains("consultant") && !Roles.Contains("sales"))
            {
                AddAdminMenu();
            }
            if (Roles.Contains("sales"))
            {
                AddSalesMenu();
            }
        }

        public string GetUserRole()
        {
            if (Roles.Contains("sales"))
            {
                return "sales";
            }

            return Roles.JoinStrings(",");
            

        }
        
        public IList<string> Roles { get;  set; }
        public IList<MenuItem> TopMenu { get; private set; }
        public IList<SideBarMenuItem> SideBarMenu { get; private set; }

        public bool HasTopMenu
        {
            get
            {
                return TopMenu != null && TopMenu.Count > 0;
            }
        }

        public bool HasSideBarMenu
        {
            get
            {
                return SideBarMenu != null && SideBarMenu.Count > 0;
            }
        }

        public string UserId { get; set; }

        private void AddSalesMenu()
        {
            TopMenu.Add(new MenuItem()
            {
                ActionName = "Index",
                ControllerName = "Client",
                DisplayName = "Kunder"
            });

            TopMenu.Add(new MenuItem()
            {
                ActionName = "Index",
                ControllerName = "Budget",
                DisplayName = "Budget"
            });

            TopMenu.Add(new MenuItem()
            {
                ActionName = "LogOff",
                ControllerName = "Account",
                DisplayName = "Log Out"
            });
        }

        private void AddAdminMenu()
        {
            TopMenu.Add(new MenuItem()
            {
                ActionName = "Index",
                ControllerName = "Client",
                DisplayName = "Kunder"
            });

            TopMenu.Add(new MenuItem()
            {
                ActionName = "Index",
                ControllerName = "Chat",
                DisplayName = "Chat"
            });

            TopMenu.Add(new MenuItem()
            {
                ActionName = "Index",
                ControllerName = "Budget",
                DisplayName = "Budget"
            });

            TopMenu.Add(new MenuItem()
            {
                ActionName = "Index",
                ControllerName = "AccountManager",
                DisplayName = "Kontoansvarig"
            });

            TopMenu.Add(new MenuItem()
            {
                ActionName = "LogOff",
                ControllerName = "Account",
                DisplayName = "Log Out"
            });
        }

        public void AddReportingMenu(bool hasCustomEvent=true)
        {
            SideBarMenu.Add(new SideBarMenuItem
            {
                Icon = "/Content/icon/menu/overview.png",
                IconDefault= "/Content/icon/menu/overview-default.png",
                ControllerName = "report",
                ActionName = "overview",
                DisplayName = "Översikt"
            });

            var sourceSideBarMenuItem = new SideBarMenuItem
            {
                Icon = "/Content/icon/menu/source.png",
                IconDefault = "/Content/icon/menu/source-default.png",
                ControllerName = "report",
                ActionName = "source",
                DisplayName = "Källor",
            };
            SideBarMenu.Add(sourceSideBarMenuItem);

            sourceSideBarMenuItem.SubMenuItems.Add(new SideBarMenuItem
            {
                Icon = "/Content/icon/menu/organic.png",
                IconDefault = "/Content/icon/menu/organic-default.png",
                ControllerName = "report",
                ActionName = "search",
                DisplayName = "Search",
            });

            sourceSideBarMenuItem.SubMenuItems.Add(new SideBarMenuItem
            {
                Icon = "/Content/icon/menu/retargeting.png",
                IconDefault = "/Content/icon/menu/retargeting-default.png",
                ControllerName = "report",
                ActionName = "retargeting",
                DisplayName = "Retargeting",
            });

            sourceSideBarMenuItem.SubMenuItems.Add(new SideBarMenuItem
            {
                Icon = "/Content/icon/menu/display.png",
                IconDefault = "/Content/icon/menu/display-default.png",
                ControllerName = "report",
                ActionName = "display",
                DisplayName = "Display",
            });
            var contactMenuItem = new SideBarMenuItem
            {
                Icon = "/Content/icon/menu/contact.png",
                IconDefault = "/Content/icon/menu/contact-default.png",
                ControllerName = "report",
                ActionName = "contact",
                DisplayName = "Kontakter"
            };
            SideBarMenu.Add(contactMenuItem);

            contactMenuItem.SubMenuItems.Add(new SideBarMenuItem()
            {
                Icon = "/Content/icon/menu/phone.png",
                IconDefault = "/Content/icon/menu/phone-default.png",
                ControllerName = "report",
                ActionName = "phone",
                DisplayName = "Telefon"
            });

            contactMenuItem.SubMenuItems.Add(new SideBarMenuItem()
            {
                Icon = "/Content/icon/menu/email.png",
                IconDefault = "/Content/icon/menu/email-default.png",
                ControllerName = "report",
                ActionName = "mail",
                DisplayName = "Mail"
            });

            contactMenuItem.SubMenuItems.Add(new SideBarMenuItem()
            {
                Icon = "/Content/icon/menu/chat.png",
                IconDefault = "/Content/icon/menu/chat-default.png",
                ControllerName = "report",
                ActionName = "chat",
                DisplayName = "Chat"
            });

            if (hasCustomEvent)
            {
                contactMenuItem.SubMenuItems.Add(new SideBarMenuItem()
                {
                    Icon = "/Content/icon/menu/web.png",
                    IconDefault = "/Content/icon/menu/web-default.png",
                    ControllerName = "report",
                    ActionName = "custom",
                    DisplayName = "Event"
                });
            }

            SideBarMenu.Add(new SideBarMenuItem
            {
                Icon = "/Content/icon/menu/leads.png",
                IconDefault = "/Content/icon/menu/leads-default.png",
                ControllerName = "report",
                ActionName = "lead",
                DisplayName = "Leads",
            });
        }

        
    }

    public class SideBarMenuItem : MenuItem
    {
        public SideBarMenuItem()
        {
            SubMenuItems = new List<SideBarMenuItem>();
        }

        public IList<SideBarMenuItem> SubMenuItems { get; private set; }

        public string Icon { get; set; }
        public string IconDefault { get; set; }
    }

    public class MenuItem
    {
        public MenuItem(){}

        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
    }
}