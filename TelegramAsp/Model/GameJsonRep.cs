using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace BullsAndCowsGame
{
    public class GameJsonRep: IRepository<Game>
    {
        string directory;

        public GameJsonRep(string directory)
        {
            this.directory = directory + "\\Resources\\games";
            if (!Directory.Exists(this.directory)) Directory.CreateDirectory(this.directory);
        }

        public GameJsonRep():this(Directory.GetCurrentDirectory()){}

        public IEnumerable<Game> GetAll()
        {
            return null;
        }

        public /*async*/ Game Get(long id)
        {
            string filename = find(id);
            if (filename == null) return null;
            return JsonSerializer.Deserialize<Game>(File.ReadAllText(directory + $"\\{filename}.json"));
            /*using(FileStream fs=new FileStream(directory + $"\\{filename}.json", FileMode.Open))
            {
                return await JsonSerializer.DeserializeAsync<Game>(fs);
            }*/
        }

        public void Update(Game item)
        {
            string filename = find(item.CurrentPlayer.Id);
            if (filename!=null)Create(item, $"{directory}\\{filename}.json");
        }

        public void Delete(long id)
        {
            string filename = find(id);
            if (filename != null) File.Delete(directory + $"\\{filename}.json");
        }

        /*async*/ void Create(Game item, string filename)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize<Game>(item,options);
            File.WriteAllText(filename, json);
            /*using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync<Game>(fs, item, options);
            }*/
        }

        public void Create(Game item)
        {
            string filename = $"{directory}\\{item.CurrentPlayer.Id}&{item.NextTurnPlayer.Id}.json";
            if (!File.Exists(filename)) Create(item, filename);
        }

        public void Save()
        {

        }

        public string find(long id)
        {
            string sid = id.ToString();
            var filenames = Directory.GetFiles(directory).Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
            if (filenames == null) return null;
            foreach (string name in filenames)
            {
                var ids = name.Split('&');
                if (ids.Length>1)
                    if ((sid == ids[0])|| (sid == ids[1])) return name;
            }
            return null;
        }
    }
}
