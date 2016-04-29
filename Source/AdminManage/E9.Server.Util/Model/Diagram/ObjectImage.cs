
namespace NM.Diagram.Render
{
    public class ObjectImage 
    {
        public ObjectImage()
        {
            path = "";
        }

        public string addon { get; set; }
        public int image_id { get; set; }
        public int case_id { get; set; }
        public int document_id { get; set; }
        public int image_order { get; set; }
        public string image_type { get; set; }
        public string image_class { get; set; }
        public string file_name { get; set; }
        public string image_description { get; set; }
        public string filename { get; set; }
        public string path { get; set; }
    }
}
