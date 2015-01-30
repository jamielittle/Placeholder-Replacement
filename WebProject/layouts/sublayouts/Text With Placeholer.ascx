<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Text With Placeholer.ascx.cs" Inherits="WebProject.layouts.sublayouts.Text_With_Placeholer" %>
<%@ Register tagPrefix="sc" namespace="Sitecore.Web.UI.WebControls" assembly="Sitecore.Kernel" %>
<div>
    <sc:text id="scText" runat="server" />
</div>
<div>
    <sc:Placeholder runat="server" Key="child-controls" />
</div>