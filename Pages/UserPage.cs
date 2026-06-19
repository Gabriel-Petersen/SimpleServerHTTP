using System.Text;
using FirstSimpleServerHTTP.Server;
using FirstSimpleServerHTTP.Models;

namespace FirstSimpleServerHTTP.Pages;

public class UserPage : DynamicPage
{
    public override byte[] Get(SortedList<string, string> parameters)
    {
        if (parameters.ContainsKey("user") == false || User.UserList.TryGetValue(parameters["user"], out User? user) == false)
        {
            HtmlModel = File.ReadAllText(HttpServer.GetFilePath("/userSearch.html"));
            if (parameters.ContainsKey("user"))
            {
                HtmlModel = HtmlModel.Replace("</body>", "<h2>Usuário não encontrado</h2></body>");
            }
        }
        else
        {
            StringBuilder infoPublica = new();
            StringBuilder infoParticular = new();

            infoPublica.Append($"<li>Nome: {user.Name}</li>");
            infoPublica.Append($"<li>Cidade: {user.City}</li>");

            infoParticular.Append($"<li>Id: {user.Id}</li>");
            infoParticular.Append($"<li>Ano de nascimento: {user.BirthDate}</li>");

            HtmlModel = HtmlModel.Replace("{{InfoPubli}}", infoPublica.ToString());
            HtmlModel = HtmlModel.Replace("{{InfoParti}}", infoParticular.ToString());
        }

        return Encoding.UTF8.GetBytes(HtmlModel);
    }
}