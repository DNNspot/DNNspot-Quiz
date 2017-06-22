<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Quiz.ascx.cs" Inherits="DNNspot.Quiz.Quiz" %>

<lo class="dnnspotQuiz">
<script>
    function print() {
        jQuery('div.passCertificate').printElement({ pageTitle: 'Continuing Education Certificate', printMode: 'popup', leaveOpen: true, overrideCSS: ['/DesktopModules/DNNspot-Quiz/print.css'] });
    }
    function putDate() {
        var date = new Date();
        date = '' + (date.getMonth() + 1) + '/' + (date.getDate()) + '/' + date.getFullYear();
        jQuery('#currentDate').html(date);
    }
</script>



<asp:Panel runat="server" ID="pnlQuizAlreadyTaken" Visible="false">    
    <h2>You have already taken this quiz</h2>
</asp:Panel>

<asp:Panel runat="server" ID="pnlTakeQuiz">    

    <h3 class="quizName"><%= _quiz.Name %></h3>

    <ul class="captureFields">
        <% foreach(var f in _quiz.CaptureFields) { %>
            <li>
                <label><%= f.Name.ToTitleCase() %></label>
                <input type="text" name="<%= CaptureFieldPrefix + f.Name %>" placeholder="<%= f.Placeholder %>" class="<%= f.IsRequired ? "required" : string.Empty %>" width="200px" />
            </li>
        <% } %>
    </ul>

    <ol class="questions">
        <% int qnum = 0; foreach(var q in _quiz.Questions) { %>
        <li>
            <p><%= q.Text %></p>
            <ol>
                <% int cnum = 0; foreach(var c in q.Choices) { %>
                    <li><label class="liLabel"><input type="radio" name="question<%= qnum %>" value="<%= c.Text %>" /><span class="answer" style="width:90%;"><%= c.Text %></span></label></li>
                <% cnum++; } %>
            </ol>
        </li>
        <% qnum++; } %>
    </ol>

    <asp:Button runat="server" ID="btnSubmit" Text="Submit" OnClick="btnSubmit_Click" />
</asp:Panel>

<asp:Panel runat="server" ID="pnlQuizResults" Visible="false" CssClass="quizResults">

    <h2>Quiz Results for "<%= _results.Quiz.Name %>"</h2>

    <% if(_quiz.DisplayScore) { %>
    <h2>Score: <%= _results.PercentScore%>%</h2>
    <% } %>

    <% if(_results.IsPassingScore) { %>
    <h4 class="passedHeader">Congratulations, you passed!</h4>
    <div class="passedMessages"><%= _results.Actions.Where(a => a.Condition == QuizCondition.QuizPassed).Select(a => a.Message).ToList().ToDelimitedString(" ")%></div>
    <% } else { %>
    <h4 class="failedHeader">Sorry, there were too many incorrect answers.</h4>   

    <ol class="questions">
        <% int qnum = 0; foreach(var q in _results.Quiz.Questions) { %>
        <li class="<%= q.IsCorrect ? "" : "error" %>">            
                <p style="<%= q.IsCorrect ? "" : "color: Red;" %>"><span><span style="font-weight: bold;"><%= q.IsCorrect ? "Correct: " : "Incorrect: " %></span><%= q.Text %></span></p>
                <ol>
                    <% int cnum = 0; foreach(var c in q.Choices) { %>
                        <li><label class="liLabel"><input type="radio" name="question<%= qnum %>" value="<%= c.Text %>" <%= (q.IsCorrect && c.IsSelected) ? "checked='checked'" : "" %> /><span class="answer" style="width:90%;"><%= c.Text %></span></label></li>
                    <% cnum++; } %>
                </ol>
            <% if(!q.IsCorrect && _quiz.DisplayHints) { 
                foreach(var m in q.IncorrectMessages) { %>
                   <div class="incorrectMessage">
                        <%= m.Text %>
                   </div> 
                <% }
            } %>            
        </li>
        <% qnum++; } %>
    </ol>
    
    <asp:Button runat="server" ID="btnSubmitAgain" Text="Submit" OnClick="btnSubmit_Click" />
    <% } %>

</asp:Panel>

<script type="text/javascript">
    jQuery(function ($) {
        $('form').validate();
    });
</script>
