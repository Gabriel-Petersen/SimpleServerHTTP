using System.Text;

namespace FirstSimpleServerHTTP.Pages;
public abstract class DynamicPage
{
    public required string HtmlModel { get; set; }

    public virtual byte[] Get(SortedList<string, string> parameters)
    {
        return Encoding.UTF8.GetBytes(HtmlModel);
    }
    
    public virtual byte[] Post(SortedList<string, string> parameters)
    {
        return Encoding.UTF8.GetBytes(HtmlModel);
    }
}