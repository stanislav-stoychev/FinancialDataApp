namespace Events.Models;

public class Publisher
{
    public string name { get; set; }

    public string homepage_url { get; set; }

    public string logo_url { get; set; }

    public string favicon_url { get; set; }
}

public class Content
{
    public string id { get; set; }

    public Publisher publisher { get; set; }

    public string title { get; set; }

    public string author { get; set; }

    public DateTime published_utc { get; set; }

    public string article_url { get; set; }

    public List<string> tickers { get; set; }

    public string image_url { get; set; }

    public string description { get; set; }

    public List<string> keywords { get; set; }

    public string amp_url { get; set; }
}