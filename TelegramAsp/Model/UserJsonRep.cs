using BullsAndCowsGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace BullsAndCowsGame
{
    public class UserJsonRep: IRepository<User>
    {
        class UserRoot
        {
            public UserRoot() { }
            public User[] users { get; set; }
        }

        string directory;

        string filename;

        public UserJsonRep(string directory,string filename= "users.json")
        {
            this.filename = filename;
            this.directory = directory + "\\Resources";
            if (!Directory.Exists(this.directory)) Directory.CreateDirectory(this.directory);
            if (!File.Exists(this.directory + '\\' + this.filename)) save((new User[0]).ToList());
        }

        public UserJsonRep():this(Directory.GetCurrentDirectory()){}

        public /*async*/ IEnumerable<User> GetAll()
        {
            /* using (FileStream fs = new FileStream(directory + '\\' + filename, FileMode.OpenOrCreate))
             {
                 var v = await JsonSerializer.DeserializeAsync<UserRoot>(fs);
                 return v.users.ToList();
             }*/
            string s = File.ReadAllText(directory + '\\' + filename);
            var v = JsonSerializer.Deserialize<UserRoot>(s);
            return v.users;
        }

        public /*async*/ User Get(long id)
        {
            //var users = await GetAll();
            var users = GetAll();
            return users.Where(x => x.Id == id).FirstOrDefault();
        }

        public async void Update(User item)
        {
           // var users = (await GetAll()).ToList();
            var users = GetAll().ToList();
            var user = users.Select(x => x).Where(x => x.Id == item.Id).FirstOrDefault();
            if (user == null) return;
            users.Remove(user);
            users.Add(item);
            save(users);
        }

        /*async*/ void save(List<User> users)
        {
            /* using (FileStream fs = new FileStream(directory + '\\' + filename, FileMode.Truncate))
             {
                 var options = new JsonSerializerOptions { WriteIndented = true };

                 await JsonSerializer.SerializeAsync<UserRoot>(fs, new UserRoot { users = users.ToArray() }, options);
             }*/
            var options = new JsonSerializerOptions { WriteIndented = true };

           string s= JsonSerializer.Serialize<UserRoot>( new UserRoot { users = users.ToArray() }, options);
            File.WriteAllText(directory + '\\' + filename, s);
        }

        public async void Create(User item)
        {
            //var users = (await GetAll()).ToList();
            var users = GetAll().ToList();
            if ((users.Select(x=>x).Where(x => x.Id == item.Id).FirstOrDefault())==null) users.Add(item);
            save(users);
        }

        public async void Delete(long id)
        {
            //var users = await GetAll();
            var users = GetAll().ToList();
            save(users.Where(x=>x.Id!=id).ToList());
        }

        public void Save()
        {

        }
    }
}
