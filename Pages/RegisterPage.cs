using System.Text;
using FirstSimpleServerHTTP.Models;

namespace FirstSimpleServerHTTP.Pages;

public class RegisterPage : DynamicPage
{
    public override byte[] Post(SortedList<string, string> parameters)
    {
        User user = new(
             User.UserList.Values.Max(u => u.Id) + 1,
            parameters["name"],
            parameters["city"],
            Convert.ToInt32(parameters["year"])
        );

        User.UserList.Add(user.Id.ToString(), user);

        string html = $"<p>Usuário cadastrado com sucesso!</p><br><p>Acesse a página <a href=\"http://localhost:8080/user.html?user={user.Id}\">AQUI</a></p>";

        return Encoding.UTF8.GetBytes(HtmlModel + html);
    }
}