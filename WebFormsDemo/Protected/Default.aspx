<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="jaytwo.AspNet.FormsAuth.WebFormsDemo.Protected.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		This page is protected.

		<p>
			jaytwo.AspNet.FormsAuth.SimpleUserProfile.Current.UserName: <%=jaytwo.AspNet.FormsAuth.SimpleUserProfile.Current.UserName %>
		</p>

		<p>
			User.Identity.Name: <%=User.Identity.Name %>
		</p>

		<asp:Button ID="btnSignOut" runat="server" Text="Sign Out" OnClick="btnSignOut_Click" />

    </div>
    </form>
</body>
</html>
