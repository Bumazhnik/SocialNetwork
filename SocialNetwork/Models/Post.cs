namespace SocialNetwork.Models{
    public class Post{
        public string Title {get;set;}
        public string Text {get;set;}
        public string Image{get;set;}
        public string Author{get;set;}
        public int Likes{get;set;}
        public int Dislikes{get;set;}
        public PostType PostType{get;set;}
    }
}