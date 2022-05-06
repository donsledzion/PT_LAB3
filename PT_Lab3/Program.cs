using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.IO;

namespace PT_Lab3
{
    class Program
    {
        public static List<Car> myCars = new List<Car>(){
                new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
                new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
                new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
                new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
                new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
                new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
                new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
                new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
                new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
            };


        static void MyCarsByModel(string _model)
        {
            var myCarsByModel = 
                    from myCar in myCars
                    where myCar.model == _model
                    select new
                    {
                        model = myCar.model,
                        engineType = (myCar.motor.model == "TDI" ? "diesel" : "petrol"),
                        hppl = (double)(myCar.motor.horsepower / myCar.motor.displacement)
                    };

            foreach(var myCarByModel in myCarsByModel)
                Console.WriteLine("CarByModel: " + myCarByModel.model + ", engineType: " + myCarByModel.engineType + ", hppl: " + myCarByModel.hppl);
            Console.WriteLine("\n========================================================\n");

            var myGroupedCars = myCarsByModel.GroupBy(c => c.engineType)
                     .Select(g => new { EngineType = g.Key, Avg = g.Average(c => c.hppl) });
            foreach(var myGroup in myGroupedCars)
            {
                Console.WriteLine("Engine Type: " + myGroup.EngineType + "| Average hppl: " + myGroup.Avg);
            }
            Console.ReadKey();
            MainMenu();
        }

        public static void SerializeToXML(string fileName, List<Car> carList)
        {
            Stream writer = new FileStream(fileName, FileMode.Create);

            CarsWrapper wrapper = new CarsWrapper(carList);
            XmlSerializer wrapperSerializer = new XmlSerializer(typeof(CarsWrapper));
            XmlSerializer serializer = new XmlSerializer(typeof(Car));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();            
            ns.Add("", "");
            wrapperSerializer.Serialize(writer, wrapper, ns);

            writer.Close();
            MainMenu();
        }

        static void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("\n========================================================\n");
            Console.WriteLine("============= WYBIERZ OPCJĘ:");
            Console.WriteLine("============= 1) Wyświetl listę samochodów");
            Console.WriteLine("============= 2) Wyświetl pogrupowaną listę samochodów");
            Console.WriteLine("============= 3) Serializacja danych");
            Console.WriteLine("============= 4) Deserializacja danych");
            Console.WriteLine("============= 5) XPath :|");
            Console.WriteLine("============= 6) LinQ :|");
            Console.WriteLine("============= 7) LinQ to XML :|");

            switch(Console.Read())
            {
                case 49:
                    DispMyCars();
                    break;
                case 50:
                    MyCarsByModel("A6");
                    break;
                case 51:
                    SerializeToXML("serializacja.xml",myCars);
                    break;
                case 52:
                    DeserializaFromFile();
                    break;
                case 53:
                    XPathSubTask();
                    break;
                case 54:
                    createXmlFromLinq(myCars);
                    break;
                case 55:
                    linq2Xml();
                    break;
            }
        }

        public static void DeserializaFromFile()
        {
            CarsWrapper newWrapper = XmlDeserialize(typeof(CarsWrapper), "inputfile.xml");

            SerializeToXML("serdeserializacja.xml",newWrapper.Cars);

        }

        public static CarsWrapper XmlDeserialize(Type dataType, string filePath)
        {
            CarsWrapper cwr = null;

            XmlSerializer xmlSerializer = new XmlSerializer(dataType);
            if(File.Exists(filePath))
            {
                TextReader textReader = new StreamReader(filePath);
                cwr = (CarsWrapper)xmlSerializer.Deserialize(textReader);
                textReader.Close();
            }

            return cwr;
        }

        static void DispMyCars()
        {
            foreach (Car car in myCars)
            {
                Console.WriteLine("Car: " + car.ToString());
            }
            Console.WriteLine("\n========================================================\n");
            Console.ReadKey();
            MainMenu();
        }

        static void XPathSubTask()
        {
            string myXPathExpression1 = "sum(//engine[@model != 'TDI']/horsepower) div count(//car)";
            string myXPathExpression2 = "//car/model[not(preceding::car/model= .)]";

            XElement rootNode = XElement.Load("inputfile.xml");
            double avgHP = (double)rootNode.XPathEvaluate(myXPathExpression1);

            IEnumerable<XElement> models =
                rootNode.XPathSelectElements(myXPathExpression2);

            
            Console.WriteLine("AvgHP(petrol): " + avgHP);
            Console.WriteLine("\n========================================================\n");
            Console.WriteLine("Distinct Models:");
            foreach (XElement model in models)
                Console.WriteLine(" - " + model.Value);
            Console.WriteLine("\n========================================================\n");
            Console.ReadKey();
            MainMenu();
        }

        private static void createXmlFromLinq(List<Car> myCars)
        {
            IEnumerable<XElement> nodes = from car in myCars
                                          select new XElement("car",
                                                    new XElement("model", car.model),
                                                    new XElement("engine", 
                                                        new XAttribute("engine", car.motor.model),
                                                        new XElement("displacement",car.motor.displacement),
                                                        new XElement("horesPower",car.motor.horsepower)
                                                        ),
                                                    new XElement("year", car.year)
                                                );

            XElement rootNode = new XElement("cars", nodes); //create a root node to contain the query results
            rootNode.Save("CarsFromLinq.xml");
        }

        private static void linq2Xml()
        {
            XDocument xmlDocument = XDocument.Load("emptyTemplate.xhtml");


            XElement result = new XElement("body",                        
                        new XElement("table", new XAttribute("border",1),
                            new XElement("tbody",
                                from car in myCars
                                select new XElement("tr",
                                    new XElement("td", car.model),
                                    new XElement("td", car.motor.model),
                                    new XElement("td", car.motor.displacement),
                                    new XElement("td", car.motor.horsepower),
                                    new XElement("td", car.year)
                                    )))
                );

            foreach (XElement element in result.Elements())
                element.RemoveAllNamespaces();

            xmlDocument.Root.Add(result);
            foreach(var node in xmlDocument.Root.Descendants().Where(n => n.Name.NamespaceName == ""))
            {
                node.Attributes("xmlns").Remove();
                // Inherit the parent namespace instead
                node.Name = node.Parent.Name.Namespace + node.Name.LocalName;
            }
            

            xmlDocument.Save("emptyTemplateOut.xhtml");
        }

        

        static void Main(string[] args)
        {
            while(true)
                MainMenu();
        }


    }

    [XmlRoot("cars")]
    public class CarsWrapper
    {
        public CarsWrapper(List<Car> _cars)
        {
            cars = _cars;
        }

        public CarsWrapper(){}

        private List<Car> cars = new List<Car>();
        [XmlElement("car")]
        public List<Car> Cars { get { return cars; } }
    }

    public static partial class Extensions
    {
        /// <summary>
        ///     An XElement extension method that removes all namespaces described by @this.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>An XElement.</returns>
        public static XElement RemoveAllNamespaces(this XElement @this)
        {
            return new XElement(@this.Name.LocalName,
                (from n in @this.Nodes()
                 select ((n is XElement) ? RemoveAllNamespaces(n as XElement) : n)),
                (@this.HasAttributes) ? (from a in @this.Attributes() select a) : null);
        }
    }
}
