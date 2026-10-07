using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkSimulation
{
    // Структура коментаря
    struct Comment
    {
        public string AuthorNickname;
        public string Text;

        public Comment(string authorNickname, string text)
        {
            AuthorNickname = authorNickname;
            Text = text;
        }
    }

    class Post
    {
        public User Author { get; }
        public DateTime CreatedAt { get; }
        public string Text { get; }
        public int Likes { get; set; }
        public List<Comment> Comments { get; } = new List<Comment>();

        public Post(User author, string text, DateTime createdAt)
        {
            Author = author;
            Text = text;
            CreatedAt = createdAt;
        }

        public void Print()
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine($"Автор: {Author.Nickname}");
            Console.WriteLine($"Дата: {CreatedAt:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"Текст: {Text}");
            Console.WriteLine($"Лайків: {Likes}");
            Console.WriteLine($"Коментарі ({Comments.Count}):");
            foreach (var c in Comments)
                Console.WriteLine($"   {c.AuthorNickname}: {c.Text}");
        }
    }

    class User
    {
        public string Name { get; }
        public string Email { get; }
        public string Nickname { get; }
        public List<Post> Posts { get; } = new List<Post>();

        public User(string name, string email, string nickname)
        {
            Name = name;
            Email = email;
            Nickname = nickname;
        }

        // Створити собі новий пост
        public Post CreatePost(string text, DateTime? date = null)
        {
            var post = new Post(this, text, date ?? DateTime.Now);
            Posts.Add(post);
            Console.WriteLine($"{Nickname} створив(ла) новий пост.");
            return post;
        }

        // Видалити свій пост
        public bool DeletePost(Post post)
        {
            if (post.Author != this)
            {
                Console.WriteLine($"{Nickname} не може видалити чужий пост.");
                return false;
            }
            bool removed = Posts.Remove(post);
            if (removed)
                Console.WriteLine($"{Nickname} видалив(ла) пост.");
            return removed;
        }

        // Отримати останні 5 постів іншого користувача
        public List<Post> GetLastPosts(User other, int count = 5)
        {
            return other.Posts
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToList();
        }

        public void PrintLastPosts(User other, int count = 5)
        {
            Console.WriteLine($"\n{Nickname} переглядає останні {count} постів {other.Nickname}:");
            var posts = GetLastPosts(other, count);
            if (posts.Count == 0)
                Console.WriteLine("Постів немає.");
            foreach (var p in posts)
                p.Print();
        }

        // Лайк посту іншого користувача
        public bool Like(Post post)
        {
            if (post.Author == this)
            {
                Console.WriteLine($"{Nickname} не може лайкати власний пост.");
                return false;
            }
            post.Likes++;
            Console.WriteLine($"{Nickname} поставив(ла) лайк посту {post.Author.Nickname}.");
            return true;
        }

        // Коментар до посту іншого користувача
        public bool WriteComment(Post post, string text)
        {
            if (post.Author == this)
            {
                Console.WriteLine($"{Nickname} не може коментувати власний пост.");
                return false;
            }
            post.Comments.Add(new Comment(Nickname, text));
            Console.WriteLine($"{Nickname} прокоментував(ла) пост {post.Author.Nickname}.");
            return true;
        }
    }

    class Program
    {
        static List<User> CreateUsers()
        {
            return new List<User>
            {
                new User("Олена Коваленко", "olena@mail.com", "@olena_k"),
                new User("Андрій Шевченко", "andrii@mail.com", "@andriy_sh"),
                new User("Марія Бондар", "maria@mail.com", "@maria_b")
            };
        }

        static void FillPosts(User user, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                // Різні дати: чим більше i, тим старіший пост
                user.CreatePost($"Пост №{i} від {user.Nickname}", DateTime.Now.AddDays(-(count - i)));
            }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var users = CreateUsers();
            var olena = users[0];
            var andriy = users[1];
            var maria = users[2];

            Console.WriteLine("=== Створення постів ===");
            FillPosts(olena, 7);
            FillPosts(andriy, 3);

            Console.WriteLine("\n=== Перегляд останніх 5 постів ===");
            maria.PrintLastPosts(olena);

            Console.WriteLine("\n=== Лайки та коментарі ===");
            var postToReact = maria.GetLastPosts(olena)[0]; // найновіший пост Олени
            maria.Like(postToReact);
            andriy.Like(postToReact);
            maria.WriteComment(postToReact, "Класний пост!");
            andriy.WriteComment(postToReact, "Згоден!");
            olena.Like(postToReact); // спроба лайкнути свій пост

            Console.WriteLine("\n=== Новий пост ===");
            maria.CreatePost("Мій перший пост у цій мережі!");

            Console.WriteLine("\n=== Видалення поста ===");
            var mariaPost = maria.Posts[0];
            andriy.DeletePost(mariaPost); // чужий пост, не вийде
            maria.DeletePost(mariaPost);  // свій пост, вийде

            Console.WriteLine("\n=== Результат ===");
            andriy.PrintLastPosts(olena);
        }
    }
}
