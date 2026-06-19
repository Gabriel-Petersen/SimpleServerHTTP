namespace FirstSimpleServerHTTP.Models;
public class User(int id, string name, string city, int birthDate)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string City { get; set; } = city;
    public int BirthDate { get; set; } = birthDate;
    public static Dictionary<string, User> UserList { get => userList; set => userList = value; }
    private static Dictionary<string, User> userList = [];

    static User()
    {
        UserList.Add("0", new(0, "João", "São Paulo", 1980));
        UserList.Add("1", new(1, "Paulo", "Minas Gerais", 1995));
        UserList.Add("2", new(2, "Clarisse", "Rio de Janeiro", 2003));
    }
}