namespace FinalProjectReservationSystems.Helpers
{
    public static class FileManager
    {
        public static string SaveFile(string path, string folderName, IFormFile formFile)
        {
            string name = formFile.FileName;

            if (name.Length > 64)
            {
                name = name.Substring(name.Length - 64, 64);
            }

            name = Guid.NewGuid().ToString() + name;


            string savePath = Path.Combine(path, folderName, name);

            using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
            }

            return name;
        }

        public static void DeleteFile(string path, string folderName, string fileName)
        {
            string deletePath = Path.Combine(path, folderName, fileName);

            if (System.IO.File.Exists(deletePath))
            {
                System.IO.File.Delete(deletePath);
            }
        }

    }
}
