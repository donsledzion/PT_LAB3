using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PT_Lab3
{
    public class Engine
    {
        [XmlAttribute]
        public string model;
        public double displacement;
        public double horsepower;

        public Engine(double _displacement, double _horsepower, string _model)
        {            
            model = _model;
            displacement = _displacement;
            horsepower = _horsepower;
        }

        public Engine()
        {
            displacement = 0f;
            horsepower = 0f;
            model = "horse";
        }


        public override string ToString()
        {
            return "Disp: " + displacement + " HP: " + horsepower + ", " + model;
        }
    }
}
