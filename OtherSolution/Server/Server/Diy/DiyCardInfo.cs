using MongoDB.Bson;


public class DiyCardInfo
{
    public ObjectId _id;
    public string UpLoadUID { get; set; }
    public string UpLoadName { get; set; }
    public DateTime UpLoadTime { get; set; }

    public string CardName { get; set; }
    public int Point { get; set; }
    public string CardType { get; set; }
    public List<String> Tag { get; set; }=new List<string>();
    public string Rank { get; set; }
    public string Camp { get; set; }
    public string Describe { get; set; }
    public string Ability { get; set; }
    public string ImageUrl { get; set; }
    public List<string> LikeUidList { get; set; } = new List<string>();
    public List<string> DislikeUidList { get; set; } = new List<string>();
    public List<Commit> Commits { get; set; } = new List<Commit>();

    public void SetLike(string uid)
	{
		if (!LikeUidList.Contains(uid))
		{
            LikeUidList.Add(uid);
        }
        DislikeUidList.Remove(uid);
	}
    public void SetDislike(string uid)
    {
        if (!DislikeUidList.Contains(uid))
        {
            DislikeUidList.Add(uid);
        }
        LikeUidList.Remove(uid);
    }
    public class Commit
    {
        public string User { get; set; }
        public string Text { get; set; }
        public int RefUid { get; set; }
        public List<string> LikeUidList { get; set; } = new List<string>();
        public List<string> DislikeUidList { get; set; } = new List<string>();
        public void SetLike(string uid)
        {
            if (!LikeUidList.Contains(uid))
            {
                LikeUidList.Add(uid);
            }
            DislikeUidList.Remove(uid);
        }
        public void SetDislike(string uid)
        {
            if (!DislikeUidList.Contains(uid))
            {
                DislikeUidList.Add(uid);
            }
            LikeUidList.Remove(uid);
        }
    }
}