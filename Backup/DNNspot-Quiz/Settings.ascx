<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="DNNspot.Quiz.Settings" %>
<div class="dnnForm dnnFormItem">
    <h2>
        Quiz Settings</h2>
    <ol class="form labelsLeft">
        <li>
            <label>
                Quiz File:</label>
            <span style="float: none;">
                <asp:DropDownList runat="server" ID="ddlQuizFile" AutoPostBack="True" OnSelectedIndexChanged="ddlQuizFile_IndexChanged">
                </asp:DropDownList>
            </span></li>
<%--        
*************NOT SURE WHY THIS WAS EXCLUDED IN THE LIVE MODULE...SAVED IT JUST IN CASE. NOT SURE. HMM....
<li>
            <label>
                Quiz Filename:</label>
            <span>
                <asp:TextBox runat="server" ID="txtFileName"></asp:TextBox>
            </span></li>--%>
        <li>
            <label>
                File Contents:</label>
            <span>
                <asp:TextBox runat="server" ID="txtFileEditor" TextMode="MultiLine" Style="width: 450px;
                    height: 350px;"></asp:TextBox>
            </span></li>
        <li>
            <label>
                &nbsp;</label>
            <span>
                <asp:Button runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" CssClass="" />
            </span></li>
    </ol>
</div>
