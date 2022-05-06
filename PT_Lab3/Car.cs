using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PT_Lab3
{
    [XmlType(TypeName ="car")]
    public class Car
    {
        public string model;
        [XmlElement("engine")]
        public Engine motor;
        public int year;


        public Car(string _model, Engine _engine, int _year)
        {
            model = _model;
            motor = _engine;
            year = _year;
        }

        public Car(){}


        public override string ToString()
        {
            return "Model: " + model + ", [" + motor.ToString() + "], year: " + year;
        }

        public static XElement Serialize(Car car)
        {
            return new XElement("car", car);
        }
    }
}
