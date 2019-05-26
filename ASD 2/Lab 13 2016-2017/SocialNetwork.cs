using System;   //  dodać referencje  System
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace lab13B
{
   [Serializable] class SocialNetwork: ISerializable
    {
        private string name;
        private List<Group> groups;

        public List<Group> Segments
        {
            get { return groups; }
        }

        public string Name
        {
            get { return name; }
        }
      
        public SocialNetwork(string name)
        {
            this.name = name;
            groups = new List<Group>();
        }

        public static SocialNetwork ReadFromFolder(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            SocialNetwork sn = new SocialNetwork(dir.Name);
            foreach(var d in dir.EnumerateDirectories())
            {
                sn.groups.Add(Group.ReadFromFolder(d.FullName));
            }
            return sn;  // zmienić
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var g in groups)
            {
                if (g == null) Console.WriteLine("inba");
                //Console.WriteLine("wypisuje " + g.Persons.Count());
                sb.Append("Group: " + g.Name + "\n\n");

                foreach (var p in g.Persons)
                {
                    sb.Append(p.ToString());
                    //Console.WriteLine(p);
                }
                sb.Append("\n");
                //Console.WriteLine("koniec grupy" + groups.Count());
            }
            return sb.ToString();
        }
        public void WriteToFolder(string path)
        {
            // etap 4
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Console.WriteLine("serialization started");
            info.AddValue("tab",groups,typeof(List<Group>) );
            info.AddValue("name", name, typeof(string));
        }

        public SocialNetwork(SerializationInfo info, StreamingContext context)
        {
            Console.WriteLine("serialization ended");
            groups = (List<Group>)info.GetValue("tab", typeof(List<Group>));
            name = (string)info.GetValue("name",typeof(string) );
            Console.WriteLine("tescik " + this.name + " 2 " + groups.Count() );
            //info.AddValue("tab",groups)
        }
        // można dopisywać niezbędne metody

    }

   [Serializable] class Group : ISerializable, IDeserializationCallback
    {
        private const string RELATIONS_FILENAME = "relacje.txt";

        public class PersonsComparer : IComparer<Person>
        {
            public int Compare(Person x, Person y)
            {
                if (x.Id > y.Id) return 1;
                if (x.Id == y.Id) return 0;
                return -1;
            }
        }

        private string name;
        private List<Person> persons;

        public List<Person> Persons
        {
            get { return persons; }
        }

        public string Name
        {
            get { return name; }
        }

        public Group(string name)
        {
            this.name = name;
            persons = new List<Person>();
        }

        public static Group ReadFromFolder(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            Group g = new Group(di.Name);
            foreach(var f in di.EnumerateFiles())
            {
                if (f.Name != RELATIONS_FILENAME)
                {
                    g.persons.Add(Person.ReadFromFile(f.FullName));
                }
            }
            foreach (var f in di.EnumerateFiles())
            {
                if (f.Name == RELATIONS_FILENAME)
                {
                    StreamReader sr = new StreamReader(f.FullName);
                    while (!sr.EndOfStream)
                    {
                        var parts = sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        int a = int.Parse(parts[0]); int b = int.Parse(parts[1]);
                        g.persons.Where(x => x.Id == a).Single().Friends.Add(b);
                        g.persons.Where(x => x.Id == b).Single().Friends.Add(a);
                    }
                    sr.Close();
                }
            }
            return g;  // zmienić
        }


        public void WriteToFolder(string path)
        {
           // etap 4
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Console.WriteLine("innerserialization start");
            info.AddValue("name2", name, typeof(string));
            info.AddValue("tab2", persons, typeof(List<Person>) );
        }

        public void OnDeserialization(object sender)
        {
            Console.WriteLine("Deserializacja grupy" + persons[0] );
        }

        public Group(SerializationInfo info, StreamingContext context)
        {
            Console.WriteLine("group deserialization end");
            persons = (List<Person>)info.GetValue("tab2", typeof(List<Person>));
            name = (string)info.GetValue("name2", typeof(string));
            Console.WriteLine("test 2 " + name + ", amount of persons " + persons.Count() + " ");
        }
        // można dopisywać niezbędne metody

    }

  [Serializable]  class Person : ISerializable, IDeserializationCallback
    {
        public override string ToString()
        {
            string s =  $"{Id} {FirstName} {LastName} {Average} \nFriends";
            foreach (int x in Friends)
            {
                s += $" {x}, ";
            }
            return s + "\n";
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Average { get; set; }

        HashSet<int> friends;

        public Person()
        {
            friends = new HashSet<int>();
        }

        public HashSet<int> Friends { get { return friends; } }

        public static Person ReadFromFile(string path)
        {
            Person p = new Person();
            StreamReader sr = new StreamReader(path);
            p.Id = int.Parse(sr.ReadLine());
            p.FirstName = sr.ReadLine();
            p.LastName = sr.ReadLine();
            p.Average = double.Parse(sr.ReadLine());
            sr.Close();
        
            return p;  // zmienić
        }

        public void WriteToFile(string path)
        {
            // etap 4
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Console.WriteLine("person starten");
            info.AddValue("name2", FirstName, typeof(string));
        }

        public void OnDeserialization(object sender)
        {
            Console.WriteLine("Deserializacja" + FirstName);
        }

        public Person(SerializationInfo info, StreamingContext context)
        {
            Console.WriteLine("person deserialization end");
            FirstName = (string)info.GetValue("name2", typeof(string));
            Console.WriteLine(FirstName);
            friends = new HashSet<int>();
            LastName = "xd";
            Id = 5;
            Average = 3.0;
        }

        // można dopisywać niezbędne metody

    }
}
