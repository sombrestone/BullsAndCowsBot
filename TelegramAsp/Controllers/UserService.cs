using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BullsAndCowsGame
{
    public class UserService
    {
        IRepository<User> repository;
        

        public UserService()
        {
            repository= new UserJsonRep();
        }

        public async Task<User> Get(long id)
        {
            //return await repository.Get(id);
            return repository.Get(id);
        }

        public async Task<User> Get(string username)
        {
            //return (await repository.GetAll()).Where(x => x.UserName.ToLower() == username.ToLower()).FirstOrDefault();
            return repository.GetAll().Where(x => x.UserName.ToLower() == username.ToLower()).FirstOrDefault();
        }

        public User UserConstructor(long id, string username)
        {
            return new User
            {
                Id = id,
                UserName = username,
                Wins = 0,
                Invites = new string[0],
                Game = ""
            };
        }

        public void NewUser(long id,string username)
        {
            repository.Create(UserConstructor(id,username));
            repository.Save();
        }

        public void UpdateUser(User item)
        {
            repository.Update(item);
            repository.Save();
        }

        public async Task<bool> AddInvite(string sender, string addressee)
        {
            User user = await Get(addressee);
            if (user == null) return false;
            if (user.Invites.Any(x => x == sender)) return false;
            var list = user.Invites.ToList();
            list.Add(sender);
            user.Invites = list.ToArray();
            UpdateUser(user);
            return true;
        }

        public async Task<bool> RemoveInvite(string addressee, string sender)
        {
            User user = await Get(addressee);
            if (user == null) return false;
            if (!user.Invites.Any(x => x == sender)) return false;
            var list = user.Invites.ToList();
            list.Remove(sender);
            user.Invites = list.ToArray();
            UpdateUser(user);
            return true;
        }

        public async Task<bool> AcceptInvite(string addressee, string sender)
        {
            if ((!(IsUserFree(sender).Result))||(!(IsUserFree(addressee).Result))) return false;
            bool result1 = await RemoveInvite(addressee, sender);
            NewGame(addressee, sender);
            return true;
        }

        public async Task<bool> IsUserFree(string username)
        {
            User user = await Get(username);
            if (user == null) return false;
            return (user.Game == "");
        }

        async void NewGame(string player1, string player2)
        {
            User u1 = await Get(player1);
            User u2 = await Get(player2);
            u1.Game = u2.UserName;
            u2.Game = u1.UserName;
            UpdateUser(u1);
            UpdateUser(u2);
        }

        public async void EndGame(string winner,string loh)
        {
            User u1 = await Get(winner);
            User u2 = await Get(loh);
            u1.Game = "";
            u1.Wins++;
            u2.Game = "";
            UpdateUser(u1);
            UpdateUser(u2);
        }

        public string Status(string username)
        {
            User user = Get(username).Result;
            string status = (user.Game == "") ? "Не в игре" : "В игре";
            string result = $"Ник: {user.UserName}\nСтатус: {status}\nПобеды: {user.Wins}\nПриглашения:";
            foreach(string invite in user.Invites)
            {
                result += $" \n{invite}";
            }
            return result;
        }
    }
}
