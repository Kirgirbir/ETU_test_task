using System.Xml.Linq;

namespace ETU_test_task
{
    public class EquipmentNode 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<EquipmentNode> Children { get; set; } = new();
    }

    public class EquipmentLogic
    {
        //документ, из которого читаем инфу
        private readonly XDocument _document;

        public EquipmentLogic(string path_to_the_file) 
        {
            if (!(File.Exists(path_to_the_file))) { 
                throw new FileNotFoundException("XML file not found!"); 
            }

            _document = XDocument.Load(path_to_the_file);
        }

        public string? IdFromPath(string path) {
            Console.WriteLine(path);

            XElement? currentElement = _document.Root;

            var path_parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in path_parts) {
                currentElement = currentElement?.Elements("node")
                    .FirstOrDefault(x => (string?)x.Attribute("name") == part);
                if (currentElement == null) return null;
            }
            return currentElement?.Attribute("id").Value;
        }
    
    }
}
