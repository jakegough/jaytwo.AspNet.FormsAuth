<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="jaytwo.AspNet.FormsAuth.WebFormsDemo.Login.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		UserName: <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox> <asp:Button ID="btnGo" runat="server" Text="Go" OnClick="btnGo_Click" />
    </div>
    </form>
</body>
</html>
