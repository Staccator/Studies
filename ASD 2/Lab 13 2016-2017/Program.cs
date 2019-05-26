using System;

namespace lab13B
{
    class Program
    {
        static void Main(string[] args)
        {
            SocialNetwork network = SocialNetwork.ReadFromFolder("MiNI");
            Console.Write(network.ToString());

            NetworkSerializers.SerializeBinary(network, "MiNI.bin");
            SocialNetwork network2 = NetworkSerializers.DeserializeBinary("MiNI.bin");
            if (network.ToString() == network2.ToString())
            {
                Console.WriteLine("Serializacja binarna - OK");
            }
            else
            {
                Console.WriteLine("Serializacja binarna - FAILED");
            }

            NetworkSerializers.SerializeSOAP(network, "MiNI.soap");
            SocialNetwork network3 = NetworkSerializers.DeserializeSOAP("MiNI.soap");
            if (network.ToString() == network3.ToString())
            {
                Console.WriteLine("Serializacja SOAP - OK");
            }
            else
            {
                Console.WriteLine("Serializacja SOAP - FAILED");
            }

            network.WriteToFolder("newFolder");
            SocialNetwork network4 = SocialNetwork.ReadFromFolder("newFolder/MiNI");
            if (network.ToString() == network4.ToString())
            {
                Console.WriteLine("Reczny zapis do struktury folderow - OK");
            }
            else
            {
                Console.WriteLine("Reczny zapis do struktury folderow - FAILED");
            }
        }
    }
}
