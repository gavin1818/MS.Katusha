﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>Main Menu</h2>

    <ul>
        <li>

        
            <%=Html.ActionLink("Login","Login","Account") %>
        </li>

        <li>
            <%=Html.ActionLink("View Tasks","Index","Task") %>
        </li>


    </ul>

</asp:Content>
