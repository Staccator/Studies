using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace lab13B
{
    class NetworkSerializers
    {
        public static void SerializeBinary(SocialNetwork network, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, network);
            fs.Close();
            //etap 2
        }

        public static SocialNetwork DeserializeBinary(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            SocialNetwork sn = (SocialNetwork)bf.Deserialize(fs);
            fs.Close();
            return sn; //zmienić
        }

        public static void SerializeSOAP(SocialNetwork network, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            SoapFormatter sf = new SoapFormatter();
            sf.Serialize(fs, network);
            fs.Close();
            //etap 3
        }
        public static SocialNetwork DeserializeSOAP(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            SoapFormatter sf = new SoapFormatter();
            SocialNetwork sn = (SocialNetwork)sf.Deserialize(fs);
            fs.Close();
            return sn; //zmienić
        }
    }
}
