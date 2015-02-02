using Sitecore;
using Sitecore.Common;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Layouts;
using Sitecore.Pipelines.GetChromeData;
using Sitecore.Reflection;
using Sitecore.Resources;
using Sitecore.SecurityModel;
using Sitecore.Shell.Web.UI;
using Sitecore.StringExtensions;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI;
using Sitecore.Web.UI.PageModes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using Sitecore.Web.UI.WebControls;

namespace PlaceholderReplacement
{
[ToolboxData("<{0}:Placeholder runat=server></{0}:Placeholder>")]
  public class Placeholder72 : Placeholder, IExpandable
  {
    /// <summary>
    /// Defines the default key for a placeholder.
    /// 
    /// </summary>
    /// 
    /// <value>
    /// "placeholder"
    /// </value>
    public static string DefaultPlaceholderKey = "placeholder";
    /// <summary>
    /// The key.
    /// </summary>
    private string key = Placeholder72.DefaultPlaceholderKey;
    /// <summary>
    /// The context key.
    /// </summary>
    private string contextKey;
    /// <summary>
    /// The _meta data item id.
    /// </summary>
    private string metaDataItemId;

    /// <summary>
    /// Caching identifier (used in key generation)
    /// 
    /// </summary>
    /// 
    /// <value>
    /// The caching ID.
    /// </value>
    public override string CacheKey
    {
      get
      {
        if (string.IsNullOrEmpty(base.CacheKey))
          return this.GetQualifiedKey();
        else
          return base.CacheKey;
      }
      set
      {
        Assert.ArgumentNotNullOrEmpty(value, "value");
        base.CacheKey = value;
      }
    }

    /// <summary>
    /// Gets the context key.
    /// 
    /// </summary>
    /// 
    /// <returns/>
    public string ContextKey
    {
      get
      {
        if (this.contextKey != null)
          return this.contextKey;
        this.contextKey = "/" + this.Key;
        Stack<Placeholder72> stack = Switcher<Placeholder72, PlaceholderSwitcher>.GetStack(false);
        if (stack == null)
          return this.contextKey;
        foreach (Placeholder72 placeholder in stack)
          this.contextKey = "/" + placeholder.Key + this.contextKey;
        return this.contextKey;
      }
    }

    /// <summary>
    /// Gets or sets the key (i.e. name).
    /// 
    /// </summary>
    /// 
    /// <value>
    /// The key.
    /// </value>
    [Category("Linking")]
    [Description("Unique name identifying the placeholder on the page")]
    public string Key
    {
      get
      {
        return this.key;
      }
      set
      {
        Assert.ArgumentNotNull((object) value, "value");
        this.key = value.ToLowerInvariant();
      }
    }

    /// <summary>
    /// Gets or sets the meta data item id.
    /// 
    /// </summary>
    /// 
    /// <value>
    /// The meta data item id.
    /// </value>
    public string MetaDataItemId
    {
      get
      {
        return StringUtil.GetString(new string[1]
        {
          this.metaDataItemId
        });
      }
      set
      {
        Assert.ArgumentNotNull((object) value, "value");
        this.metaDataItemId = value;
      }
    }

    static void Placeholder()
    {
    }

    /// <summary>
    /// Renders the control handle.
    /// 
    /// </summary>
    /// <param name="reference">The reference.</param><param name="item">The item.</param>
    /// <returns>
    /// The control handle.
    /// </returns>
    [Obsolete("Use GetControlData")]
    public static string GetControlHandle(RenderingReference reference, Item item)
    {
      Assert.ArgumentNotNull((object) reference, "reference");
      Assert.ArgumentNotNull((object) item, "item");
      return Placeholder72.GetControlData(reference, item).ToJsonContainer();
    }

    /// <summary>
    /// Gets the control data.
    /// 
    /// </summary>
    /// <param name="reference">The reference.</param><param name="item">The item.</param>
    /// <returns>
    /// The chrome data.
    /// </returns>
    public static ChromeData GetControlData(RenderingReference reference, Item item)
    {
      Assert.ArgumentNotNull((object) reference, "reference");
      Assert.ArgumentNotNull((object) item, "item");
      Item obj = item;
      if (!string.IsNullOrEmpty(reference.Settings.DataSource))
      {
        if (MainUtil.IsFullPath(reference.Settings.DataSource))
        {
          if (Sitecore.Context.Database != null)
            obj = Sitecore.Context.Database.GetItem(reference.Settings.DataSource);
        }
        else
          obj = item.Axes.GetItem(reference.Settings.DataSource);
      }
      GetChromeDataArgs args = new GetChromeDataArgs("rendering", obj ?? item);
      args.CustomData["renderingReference"] = (object) reference;
      GetChromeDataPipeline.Run(args);
      return args.ChromeData;
    }

    /// <summary>
    /// Renders the control start marker.
    /// 
    /// </summary>
    /// <param name="controlId">The control id.</param><param name="data">The data.</param>
    /// <returns>
    /// The control start marker.
    /// </returns>
    public static string GetControlStartMarker(string controlId, ChromeData data)
    {
      Assert.ArgumentNotNull((object) controlId, "controlId");
      Assert.ArgumentNotNull((object) data, "data");
      return Placeholder72.GetControlStartMarker(controlId, data, true);
    }

    /// <summary>
    /// Renders the control end marker.
    /// 
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>
    /// HTML that goes after the control in page editing mode.
    /// 
    /// </returns>
    public static string GetControlEndMarker(ChromeData data)
    {
      string str = string.Empty;
      if (data != null)
        str = Placeholder72.SanitizeAttribute(data.DisplayName);
      return StringExtensions.FormatWith("<code type='text/sitecore' chromeType='rendering' kind='close' hintKey='{0}' class='scpm'></code>", new object[1]
      {
        (object) str
      });
    }

    /// <summary>
    /// Gets the start marker.
    /// 
    /// </summary>
    /// <param name="placeholderKey">The placeholder key.</param><param name="data">The data.</param><param name="selectable">if set to <c>true</c> [selectable].</param>
    /// <returns>
    /// The start marker.
    /// </returns>
    public static string GetStartMarker(string placeholderKey, ChromeData data, bool selectable)
    {
      Assert.ArgumentNotNull((object) placeholderKey, "placeholderKey");
      Assert.ArgumentNotNull((object) data, "data");
      StringBuilder stringBuilder = new StringBuilder();
      string str = Placeholder72.EscapeNonWordCharacters(placeholderKey);
      stringBuilder.AppendFormat("<code type='text/sitecore' chromeType='placeholder' kind='open' id='{0}' key='{1}' class='scpm' data-selectable='{2}'>", (object) str, (object) placeholderKey, (object) MainUtil.BoolToString(selectable));
      stringBuilder.Append(data.ToJson());
      stringBuilder.Append("</code>");
      return ((object) stringBuilder).ToString();
    }

    /// <summary>
    /// Gets the end market.
    /// 
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>
    /// The end marker.
    /// </returns>
    public static string GetEndMarker(ChromeData data)
    {
      string str = string.Empty;
      if (data != null)
        str = Placeholder72.SanitizeAttribute(data.DisplayName);
      return StringExtensions.FormatWith("<code type='text/sitecore' chromeType='placeholder' kind='close' hintName='{0}' class='scpm'></code>", new object[1]
      {
        (object) str
      });
    }

    private static string EscapeNonWordCharacters(string key)
    {
      return Regex.Replace(key, "\\W", "_");
    }

    /// <summary>
    /// Sanitizes the attribute.
    /// 
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// Sanitized html.
    /// </returns>
    private static string SanitizeAttribute(string value)
    {
      Assert.ArgumentNotNull((object) value, "value");
      return value.Replace("'", string.Empty).Replace("\"", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty);
    }

    /// <summary>
    /// Renders the control start marker.
    /// 
    /// </summary>
    /// <param name="controlId">The control id.</param><param name="data">The data.</param><param name="selectable">Defines wheter element is selectable in</param>
    /// <returns>
    /// HTML that precedes the control in page editing mode.
    /// 
    /// </returns>
    public static string GetControlStartMarker(string controlId, ChromeData data, bool selectable)
    {
      Assert.ArgumentNotNull((object) controlId, "controlId");
      Assert.ArgumentNotNull((object) data, "data");
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = Placeholder72.SanitizeAttribute(data.DisplayName);
      string str2 = data.ToJson();
      stringBuilder.Append(StringExtensions.FormatWith("<code type='text/sitecore' chromeType='rendering' kind='open' hintName='{0}' id='r_{1}' class='scpm' data-selectable='{2}'>", (object) str1, (object) controlId, (object) MainUtil.BoolToString(selectable)));
      stringBuilder.Append(str2);
      stringBuilder.Append("</code>");
      return ((object) stringBuilder).ToString();
    }

    /// <summary>
    /// Determines whether this instance can design.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <returns>
    /// <c>true</c> if this instance can design; otherwise, <c>false</c>.
    /// </returns>
    public bool CanDesign(Database database)
    {
      Assert.ArgumentNotNull((object) database, "database");
      Item placeholderItem = Client.Page.GetPlaceholderItem(this, database);
      if (placeholderItem == null)
        return true;
      if (!placeholderItem.Access.CanRead())
        return false;
      if (Sitecore.Context.PageMode.IsPageEditorEditing)
        return true;
      string list = placeholderItem["Allowed Controls"];
      if (string.IsNullOrEmpty(list))
        return true;
      else
        return Enumerable.Any<Item>(Enumerable.Select<string, Item>((IEnumerable<string>) new ListString(list), new Func<string, Item>(database.GetItem)), (Func<Item, bool>) (item => item != null));
    }

    /// <summary>
    /// Gets the placeholder ID.
    /// </summary>
    /// 
    /// <returns>
    /// The placeholder ID.
    /// </returns>
    public string GetEscapedKey()
    {
      return this.GetEscapedKey(this.GetQualifiedKey());
    }

    /// <summary>
    /// Gets the escaped placeholder key.
    /// </summary>
    /// <param name="inputKey">The key.</param>
    /// <returns>
    /// The escaped placeholder key.
    /// </returns>
    public string GetEscapedKey(string inputKey)
    {
      Assert.ArgumentNotNull((object) inputKey, "inputKey");
      return Placeholder72.EscapeNonWordCharacters(inputKey);
    }

    /// <summary>
    /// Gets the qualified key.
    /// </summary>
    /// 
    /// <returns>
    /// The qualified key.
    /// </returns>
    public string GetQualifiedKey()
    {
      string str = this.Key;
      for (Control parent = this.Parent; parent != null; parent = parent.Parent)
      {
        Placeholder placeholder = parent as Placeholder;
        if (placeholder != null && !string.IsNullOrEmpty(placeholder.Key))
        {
          if (!str.StartsWith("/", StringComparison.InvariantCulture))
            str = "/" + str;
          str = "/" + placeholder.Key + str;
        }
      }
      return str;
    }

    /// <summary>
    /// Renders the page design mode.
    /// </summary>
    /// <param name="output">The output.</param>
    private void RenderPlaceholderPageDesignMode(HtmlTextWriter output)
    {
      Assert.ArgumentNotNull((object) output, "output");
      bool flag = true;
      if (!this.CanDesign(Sitecore.Context.Database) || !Policy.IsAllowed("Page Editor/Can Design"))
        flag = false;
      Item contextItem = this.GetContextItem();
      if (contextItem != null && !WebEditUtil.CanDesignItem(contextItem))
        flag = false;
      string qualifiedKey = this.GetQualifiedKey();
      Item obj = this.GetItem();
      GetChromeDataArgs args = obj != null ? new GetChromeDataArgs("placeholder", obj) : new GetChromeDataArgs("placeholder");
      args.CustomData["placeHolderKey"] = (object) qualifiedKey;
      GetChromeDataPipeline.Run(args);
      ChromeData chromeData = args.ChromeData;
      string startMarker = Placeholder72.GetStartMarker(qualifiedKey, chromeData, flag);
      output.Write(startMarker);
      for (int index = 0; index < this.Controls.Count; ++index)
      {
        Control control = this.Controls[index];
        this.RenderControlPageDesignMode(output, flag, control);
      }
      string endMarker = Placeholder72.GetEndMarker(chromeData);
      output.Write(endMarker);
    }

    /// <summary>
    /// Renders the control page design mode.
    /// 
    /// </summary>
    /// <param name="output">The output.</param><param name="isDesignAllowed">if set to <c>true</c> [is design allowed].</param><param name="control">The control.</param>
    private void RenderControlPageDesignMode(HtmlTextWriter output, bool isDesignAllowed, Control control)
    {
      Assert.ArgumentNotNull((object) output, "output");
      Assert.ArgumentNotNull((object) control, "control");
      RenderingReference renderingReference = Client.Page.GetRenderingReference(control);
      bool flag = false;
      if (renderingReference != null)
      {
        string uniqueId = renderingReference.UniqueId;
        if (Sitecore.Data.ID.IsID(uniqueId))
        {
          string controlId = ShortID.Encode(uniqueId);
          Item obj = this.GetItem();
          Assert.IsNotNull((object) obj, "item");
          ChromeData controlData = Placeholder72.GetControlData(renderingReference, obj);
          output.Write(Placeholder72.GetControlStartMarker(controlId, controlData, isDesignAllowed));
          control.RenderControl(output);
          output.Write(Placeholder72.GetControlEndMarker(controlData));
          flag = true;
        }
      }
      if (flag)
        return;
      control.RenderControl(output);
    }

    /// <summary>
    /// Determines whether the specified placeholder qualified key matches the control key.
    /// 
    /// </summary>
    /// <param name="placeholderQualifiedKey">The placeholder qualified key.</param><param name="controlKey">The control key.</param>
    /// <returns>
    /// <c>true</c> if the specified placeholder qualified key matches the control key; otherwise, <c>false</c>.
    /// /// </returns>
    public static bool IsMatch(string placeholderQualifiedKey, string controlKey)
    {
      Assert.ArgumentNotNullOrEmpty(placeholderQualifiedKey, "placeholderQualifiedKey");
      Assert.ArgumentNotNull((object) controlKey, "controlKey");
      if (controlKey.StartsWith("/", StringComparison.OrdinalIgnoreCase))
        return string.Compare(placeholderQualifiedKey, controlKey, StringComparison.OrdinalIgnoreCase) == 0;
      else
        return string.Compare(StringUtil.GetLastPart(placeholderQualifiedKey, '/', placeholderQualifiedKey), controlKey, StringComparison.OrdinalIgnoreCase) == 0;
    }

    /// <summary>
    /// Determines whether the specified rendering is sublayout.
    /// 
    /// </summary>
    /// <param name="rendering">The rendering.</param>
    /// <returns>
    /// <c>true</c> if the specified rendering is sublayout; otherwise, <c>false</c>.
    /// 
    /// </returns>
    private static bool IsSublayout(Item rendering)
    {
      Assert.ArgumentNotNull((object) rendering, "rendering");
      bool flag = rendering.TemplateID == TemplateIDs.Sublayout;
      if (!flag)
      {
        Template template = TemplateManager.GetTemplate(rendering);
        if (template != null)
          flag = template.DescendsFrom(TemplateIDs.Sublayout);
      }
      return flag;
    }

    /// <summary>
    /// Renders the footer.
    /// 
    /// </summary>
    /// <param name="output">The output.</param>
    private static void RenderFooter(HtmlTextWriter output)
    {
      Assert.ArgumentNotNull((object) output, "output");
      output.Write("</td></tr></table>");
    }

    /// <summary>
    /// Renders the header.
    /// 
    /// </summary>
    /// <param name="output">The output.</param><param name="list">The list.</param>
    private void RenderHeader(HtmlTextWriter output, ICollection<RenderingDefinition> list)
    {
      Assert.ArgumentNotNull((object) output, "output");
      Assert.ArgumentNotNull((object) list, "list");
      string str = string.Empty;
      string qualifiedKey = this.GetQualifiedKey();
      Item placeholderItem = Sitecore.Context.Page.GetPlaceholderItem(this, Sitecore.Context.Database);
      if (placeholderItem != null)
        str = placeholderItem["Allowed Controls"];
      output.Write("<table width=\"100%\" style=\"table-layout:fixed\" border=\"0\"><tr>");
      output.Write("<td style=\"background:#cad8f0;border:1px solid #97b3e1;font:8pt tahoma;cursor:default\" ondragover='javascript:var scText=event.dataTransfer.getData(\"text\");if(scText!=null && scText.substring(0,9)==\"sitecore:\"){event.dataTransfer.dropEffect=\"copy\";event.cancelBubble=true;event.returnValue=false;return true}' ondrop='javascript:if (event.dataTransfer.getData(\"text\").substring(0,9)==\"sitecore:\"){window.parent.scForm.postEvent(this,event,\"Drop(\\\"\" + event.dataTransfer.getData(\"text\") + \"\\\",\\\"" + qualifiedKey + "\\\",\\\"" + str + "\\\")\")}' onselectstart='javascript:return false'>");
      output.Write("<div style=\"padding:4px\">");
      output.Write("<a href=\"#\" onclick=\"javascript:return window.parent.scForm.postEvent(this,event,'pagedesigner:add(placeholdername=" + qualifiedKey + ",allowedrenderings=" + str + ")')\" style=\"font:8pt tahoma;color:black;text-decoration:none}\" onmouseover=\"javascript:this.style.textDecoration='underline'\" onmouseout=\"javascript:this.style.textDecoration='none'\" title=\"" + Translate.Text("Click to add") + "\">");
      new ImageBuilder()
      {
        Src = "Software/16x16/element_selection.png",
        Width = 16,
        Height = 16,
        Align = "absmiddle",
        Margin = "0px 4px 0px 0px"
      }.Render(output);
      output.Write(qualifiedKey);
      output.Write("</a>");
      output.Write("</div>");
    }

    /// <summary>
    /// Renders the in page design mode.
    /// 
    /// </summary>
    /// <param name="output">The output.</param><param name="pageDesignerHandle">The page designer handle.</param>
    private bool RenderPageLayoutMode(HtmlTextWriter output, string pageDesignerHandle)
    {
      Assert.ArgumentNotNull((object) output, "output");
      Assert.ArgumentNotNullOrEmpty(pageDesignerHandle, "pageDesignerHandle");
      if (!this.CanDesign(Sitecore.Context.Database))
        return false;
      string sessionString = WebUtil.GetSessionString(pageDesignerHandle);
      if (string.IsNullOrEmpty(sessionString))
        return true;
      DeviceItem device1 = Sitecore.Context.Device;
      if (device1 == null)
        return false;
      DeviceDefinition device2 = LayoutDefinition.Parse(sessionString).GetDevice(device1.ID.ToString());
      string qualifiedKey = this.GetQualifiedKey();
      List<RenderingDefinition> list = new List<RenderingDefinition>();
      foreach (RenderingDefinition renderingDefinition in device2.Renderings)
      {
        if (Placeholder72.IsMatch(qualifiedKey, renderingDefinition.Placeholder))
          list.Add(renderingDefinition);
      }
      this.RenderHeader(output, (ICollection<RenderingDefinition>) list);
      this.RenderRenderings(output, list);
      Placeholder72.RenderFooter(output);
      return true;
    }

    /// <summary>
    /// Renders the rendering.
    /// 
    /// </summary>
    /// <param name="output">The output.</param><param name="rendering">The rendering.</param><param name="renderingDefinition">The rendering definition.</param><param name="index">The index.</param>
    private void RenderRendering(HtmlTextWriter output, Item rendering, RenderingDefinition renderingDefinition, int index)
    {
      Assert.ArgumentNotNull((object) output, "output");
      Assert.ArgumentNotNull((object) rendering, "rendering");
      Assert.ArgumentNotNull((object) renderingDefinition, "renderingDefinition");
      string str1 = rendering.DisplayName;
      if (rendering.ID == RenderingIDs.FieldRenderer)
      {
        string @string = StringUtil.GetString(new string[1]
        {
          WebUtil.ParseQueryString(StringUtil.GetString(new string[1]
          {
            renderingDefinition.Parameters
          }))["FieldName"]
        });
        if (!string.IsNullOrEmpty(@string))
        {
          TemplateFieldItem templateFieldItem = (TemplateFieldItem) rendering.Database.GetItem(@string);
          if (templateFieldItem != null)
            str1 = "\"" + StringUtil.GetString(new string[2]
            {
              templateFieldItem.Title,
              templateFieldItem.Name
            }) + "\" " + Translate.Text("Field");
          else
            str1 = Translate.Text("[unknown field]");
        }
        else
          str1 = Translate.Text("[unknown field]");
      }
      string str2 = string.Empty;
      string qualifiedKey = this.GetQualifiedKey();
      Item placeholderItem = Sitecore.Context.Page.GetPlaceholderItem(this, Sitecore.Context.Database);
      if (placeholderItem != null)
        str2 = placeholderItem["Allowed Controls"];
      string str3 = "javascript:return window.parent.scForm.postEvent(this,event,'pagedesigner:editplaceholder(placeholdername=" + (object) qualifiedKey + ",allowedrenderings=" + str2 + ",index=" + (string) (object) index + ")')";
      output.Write("<a href=\"#\" style=\"display:block;color:black;background:#e2ebf8;border:1px solid #97b3e1;font:8pt tahoma;padding:4px;margin:3px 0px 0px 0px;text-decoration:none\" onclick=\"" + str3 + "\" onmouseover=\"javascript:this.style.textDecoration='underline'\" onmouseout=\"javascript:this.style.textDecoration='none'\">");
      output.Write(ItemTileService.RenderTile(rendering, TileView.IconOnly, ImageDimension.id16x16));
      output.Write(' ');
      output.Write(str1);
      output.Write("</a>");
    }

    /// <summary>
    /// Renders the renderings.
    /// 
    /// </summary>
    /// <param name="output">The output.</param><param name="list">The list.</param>
    private void RenderRenderings(HtmlTextWriter output, List<RenderingDefinition> list)
    {
      Assert.ArgumentNotNull((object) output, "output");
      Assert.ArgumentNotNull((object) list, "list");
      for (int index = 0; index < list.Count; ++index)
      {
        RenderingDefinition renderingDefinition = list[index];
        string itemId = renderingDefinition.ItemID;
        if (!string.IsNullOrEmpty(itemId))
        {
          Item rendering = Sitecore.Context.Database.GetItem(itemId);
          if (rendering != null)
          {
            if (Placeholder72.IsSublayout(rendering))
              this.RenderSublayout(output, rendering, renderingDefinition, index);
            else
              this.RenderRendering(output, rendering, renderingDefinition, index);
          }
        }
      }
    }

    /// <summary>
    /// Renders the sublayout.
    /// 
    /// </summary>
    /// <param name="output">The output.</param><param name="rendering">The rendering.</param><param name="renderingDefinition">The rendering definition.</param><param name="index">The index.</param>
    private void RenderSublayout(HtmlTextWriter output, Item rendering, RenderingDefinition renderingDefinition, int index)
    {
      Assert.ArgumentNotNull((object) output, "output");
      Assert.ArgumentNotNull((object) rendering, "rendering");
      Assert.ArgumentNotNull((object) renderingDefinition, "renderingDefinition");
      foreach (Control control in this.Controls)
      {
        Sublayout sublayout = control as Sublayout;
        if (sublayout != null && string.Compare(sublayout.Path, rendering["Path"], StringComparison.InvariantCulture) == 0)
        {
          output.Write("<table width=\"100%\" style=\"table-layout:fixed;background:#e2ebf8;border:1px solid #97b3e1;margin:3px 0px 0px 0px\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\">");
          output.Write("<tr><td>");
          string str1 = rendering.DisplayName;
          if (rendering.ID == RenderingIDs.FieldRenderer)
          {
            string @string = StringUtil.GetString(new string[1]
            {
              WebUtil.ParseQueryString(renderingDefinition.Parameters)["FieldName"]
            });
            if (!string.IsNullOrEmpty(@string))
            {
              TemplateFieldItem templateFieldItem = (TemplateFieldItem) rendering.Database.GetItem(@string);
              if (templateFieldItem != null)
                str1 = "\"" + StringUtil.GetString(new string[2]
                {
                  templateFieldItem.Title,
                  templateFieldItem.Name
                }) + "\" " + Translate.Text("Field");
              else
                str1 = Translate.Text("[unknown field]");
            }
            else
              str1 = Translate.Text("[unknown field]");
          }
          string str2 = string.Empty;
          string qualifiedKey = this.GetQualifiedKey();
          Item placeholderItem = Sitecore.Context.Page.GetPlaceholderItem(this, Sitecore.Context.Database);
          if (placeholderItem != null)
            str2 = placeholderItem["Allowed Controls"];
          string str3 = "javascript:return window.parent.scForm.postEvent(this,event,'pagedesigner:editplaceholder(placeholdername=" + (object) qualifiedKey + ",allowedrenderings=" + str2 + ",index=" + (string) (object) index + ")')";
          output.Write("<a href=\"#\" style=\"display:block;color:black;font:8pt tahoma;padding:2px 2px 0px 2px;text-decoration:none\" onclick=\"" + str3 + "\" onmouseover=\"javascript:this.style.textDecoration='underline'\" onmouseout=\"javascript:this.style.textDecoration='none'\">");
          output.Write(ItemTileService.RenderTile(rendering, TileView.IconOnly, ImageDimension.id16x16));
          output.Write(' ');
          output.Write(str1);
          output.Write("</a>");
          output.Write("</td></tr>");
          output.Write("<tr><td>");
          ((Control) sublayout).RenderControl(output);
          output.Write("</td></tr></table>");
          break;
        }
      }
    }

    /// <summary>
    /// Gets the placeholders of a control.
    /// </summary>
    /// <param name="control">The control.</param><param name="includeSelf">if set to <c>true</c> [include self].</param>
    /// <returns/>
    public static List<Placeholder72> GetPlaceholders(Control control, bool includeSelf)
    {
      Assert.ArgumentNotNull((object) control, "control");
      List<Placeholder72> result = new List<Placeholder72>();
      if (includeSelf)
      {
        Placeholder72.GetPlaceholders(control, result);
      }
      else
      {
        foreach (Control control1 in control.Controls)
          Placeholder72.GetPlaceholders(control1, result);
      }
      return result;
    }

    /// <summary>
    /// Gets the gtext used when tracing.
    /// </summary>
    /// 
    /// <returns>
    /// The ID or the rendering name with the type name of the control.
    /// </returns>
    public override string GetTraceName()
    {
      return "Placeholder: " + this.Key;
    }

    /// <summary>
    /// Expands this instance.
    /// </summary>
    public void Expand()
    {
      this.EnsureChildControls();
    }

    /// <summary>
    /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
    /// </summary>
    protected override void CreateChildControls()
    {
      base.CreateChildControls();
      Nexus.PlaceholderApi.CreateChildControls(this);
    }

    /// <summary>
    /// Renders the control.
    /// </summary>
    /// <param name="output">The output.</param>
    protected override void DoRender(HtmlTextWriter output)
    {
      Assert.ArgumentNotNull((object) output, "output");
      bool flag = false;
      if (Sitecore.Context.PageDesigner.IsLayouting && !Sitecore.Context.PageDesigner.IsLayoutViewing)
          flag = this.RenderPageLayoutMode(output, Sitecore.Context.PageDesigner.LayoutHandle);
      if (Sitecore.Context.PageMode.IsPageEditorEditing)
      {
        this.RenderPlaceholderPageDesignMode(output);
        flag = true;
      }
      if (flag)
        return;
      this.RenderChildren(output);
    }

    /// <summary>
    /// Inserts a control.
    /// </summary>
    /// <param name="control">The control.</param>
    protected virtual void InsertControl(Control control)
    {
      Assert.ArgumentNotNull((object) control, "control");
      this.Controls.Add(control);
    }

    /// <summary>
    /// Inserts a rendering.
    /// </summary>
    /// <param name="reference">The reference.</param>
    protected virtual void InsertRendering(RenderingReference reference)
    {
      Assert.ArgumentNotNull((object) reference, "reference");
      Control control = reference.GetControl();
      if (control == null)
      {
        RenderingItem renderingItem = reference.RenderingItem;
        Tracer.Error((object) ("Could not instantiate control for the rendering '" + (renderingItem != null ? renderingItem.Name : "[unknown]") + "'"));
      }
      else
      {
        if (!ItemUtil.IsNull(reference.RenderingID))
        {
          WebControl webControl = control as WebControl;
          if (webControl != null && string.IsNullOrEmpty(webControl.RenderingID))
            webControl.RenderingID = StringUtil.GetString((object) reference.RenderingID);
        }
        this.InsertControl(control);
      }
    }

    /// <summary>
    /// Inserts the renderings.
    /// </summary>
    [Obsolete("Deprecated.")]
    protected virtual void InsertRenderings()
    {
      this.EnsureChildControls();
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnLoad(EventArgs e)
    {
      Assert.ArgumentNotNull((object) e, "e");
      base.OnLoad(e);
      this.EnsureChildControls();
    }

    /// <summary>
    /// Gets the placeholders of the control.
    /// 
    /// </summary>
    /// <param name="control">The control.</param><param name="result">The result.</param>
    private static void GetPlaceholders(Control control, List<Placeholder72> result)
    {
      Assert.ArgumentNotNull((object) control, "control");
      Assert.ArgumentNotNull((object) result, "result");
      if (control is Placeholder)
        result.Add(control as Placeholder72);
      else if (control is IHasPlaceholders)
      {
        List<Placeholder> placeholders = (control as IHasPlaceholders).GetPlaceholders();
        if (placeholders == null)
          return;
        result.AddRange((IEnumerable<Placeholder72>) placeholders);
      }
      else
      {
        foreach (Control control1 in control.Controls)
          Placeholder72.GetPlaceholders(control1, result);
      }
    }
  }
}