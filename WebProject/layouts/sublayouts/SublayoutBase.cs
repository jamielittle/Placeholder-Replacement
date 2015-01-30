using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.layouts.sublayouts
{
    public class SublayoutBase : System.Web.UI.UserControl
    {
        public Item DataSourceItem
        {
            get
            {
                Item returnItem = Sitecore.Context.Item;
                Sublayout controlSublayout = (this.Parent) as Sublayout;
                if (controlSublayout != null)
                {
                    string dataSourceString = controlSublayout.DataSource;

                    if (!string.IsNullOrEmpty(dataSourceString))
                    {
                        Item dbItem = Sitecore.Context.Database.GetItem(dataSourceString);

                        if (dbItem != null)
                        {
                            returnItem = dbItem;
                        }
                    }
                }

                return returnItem;
            }
        }
    }
}