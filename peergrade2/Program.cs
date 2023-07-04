using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace peergrade2
{
    //Создадим класс в котором и будут все методы файлового менеджера.
    class FileManagers
    {
        //объявим переменные класса для удобства работы.
        //path - переменная с полным путем до директориии.
        //Folder_and_files - список папок и файлов находящихся по пути path
        //OSVersion - операционная система пользователя.
        //FileName - название файла к которому применяется свойство.
        //FolderName - название папки к которому применяется свойство.
        //FolderPath - путь до каталога и название папки
        //FilePath - путь до файла и флаг на действие(перемещение или копирование).
        //MergeFiles - список файлов которые пользователь хочет объеденить.
        DirectoryInfo path;
        List<string> Folder_and_files;
        string OSVersion;
        string FileName;
        string FolderName;
        (string, string) FolderPath;
        (string, bool) FilePath = (null,true);
        List<string> MergeFiles = new List<string>();

        //создание статовой позиции, с подсказками для пользователя.
        public void start()
        {
            Console.WriteLine("Приветствую в менеджере файлов.");
            Console.WriteLine("1)Используйте стрелочки вверх/вниз для выбора нужной дирректории.");
            Console.WriteLine("2)Чтобы подтвердить выбор нажмите ENTER.");
            Console.WriteLine("3)Для доступа к большему функционалу(далее параметры) с папкой/файлом нажмите стрелку вправо.");
            Console.WriteLine("4)Для возвращения из параметров нажмите стрелку влево");
            Console.WriteLine("5)Для перехода в раздел HELP нажимайте H");
            ConsoleKeyInfo keypress;
            keypress = Console.ReadKey();
            //проверка на запрос о помощи
            if (keypress.Key == ConsoleKey.H)
                Help();
            //узнаем систему пользователя
            GetOS();
            //переход в корневой католог системы.(Для Windows это список дисков.)
            Folder_and_files = DiskList();
            WriteDirsFolders();
        }

        //реализуем функцию, выводящую подсказки пользователю.
        public void Help()
        {
            Console.Clear();
            Console.WriteLine("Вы перешли в настройки, после нажатия на любую клавишу вы вернетесь на прошлую позицию.");
            Console.WriteLine("1)Используйте стрелочки вверх/вниз для выбора нужной дирректории.");
            Console.WriteLine("2)Чтобы подтвердить выбор нажмите ENTER.");
            Console.WriteLine("3)Для доступа к большему функционалу(далее параметры) с папкой/файлом нажмите стрелку вправо.");
            Console.WriteLine("4)Для возвращения из параметров нажмите стрелку влево");
            Console.WriteLine("5)Для копирования/перемещения папок/файлов нужно зайти в параметры объекта->копировать/переместить,");
            Console.WriteLine("---перейти в дирректорию в которую нужно вставить файл, зайти в параметры от любого объекта и выбрать вставить.");
            Console.WriteLine("6)Для объединения файлов нужна зайти в параметры одного из файлов, добавить его в список объединения.");
            Console.WriteLine("---после того как будет выбранно 2 или более файлов зайдя в параметры от любого объекта появится вариант \"объединить\".");
            Console.WriteLine("---Объеденять файлы можно из разных дирректорий. После выбора \"объеденить\", нужно ввести имя файла, и файл появится в директории в которой вы находитесь.");  
            Console.WriteLine("7)Можно объеденить один и тот же файл тогда его содержимое удвоиться.");
            Console.WriteLine("8)Для удаления перейдите в параметры объекта и выберите удалить объект.");
            Console.WriteLine("ВАЖНО: при удалении каталогов с подкатологами внутри некоторые файлы могут остаться, изза доступа.");
            Console.WriteLine("9)В меню с каталогами будет выводиться ошибка, если нельзя перемещаться по данной клавише.");
          


            Console.WriteLine("Для продолжения нажмите любую кнопку.");
            Console.ReadLine();
        }

        //запись системы пользователя в OSVersion.
        public void GetOS()
        {
            string a = Environment.OSVersion.ToString();
            string[] b = a.Split(' ');
            OSVersion = b[0];
        }

        //возвращает список дисков или корневой католог, в зависимости от системы.
        public List<string> DiskList()
        {
            if (OSVersion == "Microsoft")
            {
                List<string> disks = Environment.GetLogicalDrives().ToList();
                return disks;
            }
            else
            {
                List<string> dirs_and_files = new List<string>();
                path = new DirectoryInfo("/");
                DirectoryInfo[] dirs = path.GetDirectories();
                FileInfo[] files = path.GetFiles();
                foreach(var i in dirs)
                {
                    dirs_and_files.Add(i.Name);
                }
                foreach(var i in files)
                {
                    dirs_and_files.Add(i.Name);
                }
                return dirs_and_files;
            }
            
        }

        //сделаем метод обработки нажатий на клавиши.
        public void WriteDirsFolders()
        {
            //номер отмеченной строки.
            int num_str = 0;
            while(true)
            {
                Console.Clear();
                if (path != null)
                {
                    Console.WriteLine($"Сейчас вы находитесь в {path.FullName}.");
                }
                else
                {
                    Console.WriteLine($"Сейчас вы находитесь в дисках.");
                }
                //выведем список м папками и файлами.
                for (int i = 0; i < Folder_and_files.Count(); i++)
                {
                    if (i == num_str)
                    {
                        (ConsoleColor, ConsoleColor) color = GetColor();
                        Console.BackgroundColor = color.Item1;
                        Console.ForegroundColor = color.Item2;
                        Console.WriteLine(Folder_and_files[i]);
                        Console.ResetColor();
                        continue;
                    }
                    Console.WriteLine(Folder_and_files[i]);
                }
                bool a = false;
                CheckInputBtn(ref num_str,ref a);
            }
        }
        
        //интерфейс(отклики от нажатий на клаиши).
        public void CheckInputBtn(ref int num_str,ref bool fl)
        {
            while (true)
            {
                ConsoleKeyInfo keypress = Console.ReadKey(true);
                if (keypress.Key == ConsoleKey.UpArrow && num_str - 1 >= 0)
                    num_str -= 1;
                else if (keypress.Key == ConsoleKey.DownArrow && num_str + 1 < Folder_and_files.Count())
                    num_str += 1;

                else if ((keypress.Key == ConsoleKey.Enter) && fl)
                {
                    ChooseOptions(Folder_and_files[num_str]);
                    fl = false;
                    num_str = 0;
                }

                else if ((keypress.Key == ConsoleKey.Enter) && path != null && num_str >= Folder_and_files.Count() - path.GetFiles().Length)PrintFile(Folder_and_files[num_str]);
                else if (keypress.Key == ConsoleKey.Enter) OpenFolders(Folder_and_files[num_str], ref num_str);
                else if (keypress.Key == ConsoleKey.H) Help();
                else if (keypress.Key == ConsoleKey.RightArrow && path != null && num_str >= Folder_and_files.Count() - path.GetFiles().Length && !fl)OptionsFiles(Folder_and_files[num_str]);
                else if (keypress.Key == ConsoleKey.RightArrow && !fl)
                {
                    if (OSVersion == "Microsoft" && path == null)
                    {
                        Console.WriteLine("Ошибка, не возможно перейти в параметры диска.");
                        continue;
                    }
                    OptionsFolders(Folder_and_files[num_str]);
                }

                else if (keypress.Key == ConsoleKey.LeftArrow && fl)
                    fl = false;

                else
                {
                    Console.WriteLine("Ошибка, для помощи нажмите H.");
                    continue;
                }
                return;
            }
        }

        //выбор кодировки файла.
        public System.Text.Encoding OpenFiles()
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            Console.Clear();
            bool fl = true;
            while (fl)
            {
                Console.WriteLine("Введите нужную кодировку из списка.");
                Console.WriteLine("UTF-8, UTF-7, UTF-32, ASCII, UNICODE.");

                string input = Console.ReadLine();
                switch (input.ToLower())
                {
                    case "utf-8":
                        enc = System.Text.Encoding.UTF8;
                        fl = false;
                        break;
                    case "utf-7":
                        enc = System.Text.Encoding.UTF7;
                        fl = false;
                        break;
                    case "utf-32":
                        enc = System.Text.Encoding.UTF32;
                        fl = false;
                        break;
                    case "ascii":
                        enc = System.Text.Encoding.ASCII;
                        fl = false;
                        break;
                    case "unicode":
                        enc = System.Text.Encoding.Unicode;
                        fl = false;
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Ошибка, данной кодировки нет в списке, попробуйте еще раз!");
                        break;
                }
            }
            return enc;
            
        }

        //выводит файл в консоль.
        public void PrintFile(string name)
        {
            var enc = OpenFiles();
            Console.Clear();
            string text = "";
            if (OSVersion == "Microsoft" && path.Parent != null)
                name = path.FullName + "\\" + name;
            else
                name = path.FullName + "/" + name;
            try
            {
                using (StreamReader fs = new StreamReader(name, enc))
                {
                    while (true)
                    {
                        // Читаем строку из файла во временную переменную.
                        string temp = fs.ReadLine();

                        // Если достигнут конец файла, прерываем считывание.
                        if (temp == null) break;

                        // Пишем считанную строку в итоговую переменную.
                        text += temp + "\n";
                    }
                }
                // Выводим на экран.
                Console.WriteLine(text);
            }
            catch
            {
                Console.WriteLine("Невозможно открыть файл!");
            }
            Console.WriteLine("Для перехода в каталог нажмите любую клавишу.");
            var a = Console.ReadKey();
            if (a.Key == ConsoleKey.H) Help();
        }

        //функция перемещения по каталогам.
        public void OpenFolders(string name, ref int num_str)
        {
            string a = null;
            if (path != null)
                a = path.FullName;
            try
            {
                num_str = 0;
                if (path == null)
                {
                    path = new DirectoryInfo(name);
                }
                else if (name == "...")
                {
                    path = path.Parent;
                }
                else
                {
                    if (path.Parent == null) path = new DirectoryInfo(path + name);
                    else path = new DirectoryInfo(OSVersion == "Microsoft" ? path.FullName + '\\' + name : path.FullName + '/' + name);
                }
                UpdateFolderFiles();
            }
            catch
            {
                Console.WriteLine("Нет доступа.");
                Console.WriteLine("Для продолжения нажмите на кнопку.");
                Console.ReadKey();
                if (a != null)
                {
                    path = new DirectoryInfo(a);
                    UpdateFolderFiles();
                }
            }
        }

        //свойства для файлов.
        public void OptionsFiles(string name)
        {
            FileName = name;
            bool fl = true;
            int num_str = 0;
            Console.Clear();
            Folder_and_files.Clear();
            Folder_and_files.Add("Создать каталог.");
            Folder_and_files.Add("Открыть файл.");
            Folder_and_files.Add("Создать файл.");
            Folder_and_files.Add("Удалить файл.");
            Folder_and_files.Add("Свойства файла.");
            Folder_and_files.Add("Копировать файл.");
            Folder_and_files.Add("Переместить файл.");
            Folder_and_files.Add("Добавить в список объединения файлов.");
            if(MergeFiles.Count > 1) Folder_and_files.Add("объеденить выбранные файлы");
            if(FilePath.Item1 != null) Folder_and_files.Add("вставить файл");
            if (FolderPath != (null, null)) Folder_and_files.Add("Вставить каталог.");
            while (fl)
            {
                Console.Clear();
                Console.WriteLine("Параметры файла.");
                for (int i = 0; i < Folder_and_files.Count(); i++)
                {
                    if (i == num_str)
                    {
                        (ConsoleColor, ConsoleColor) color = GetColor();
                        Console.BackgroundColor = color.Item1;
                        Console.ForegroundColor = color.Item2;
                        Console.WriteLine(Folder_and_files[i]);
                        Console.ResetColor();
                        continue;
                    }
                    Console.WriteLine(Folder_and_files[i]);
                }
                CheckInputBtn(ref num_str, ref fl);
            }
            UpdateFolderFiles();
            
        }

        //выбор цвета
        public (ConsoleColor, ConsoleColor) GetColor()
        {
            if (Console.BackgroundColor == ConsoleColor.Black) return (ConsoleColor.White, ConsoleColor.Black);
            else return (ConsoleColor.Black, ConsoleColor.White);
        }

        //свойства для папок.
        public void OptionsFolders(string name)
        {
            FolderName = name;
            bool fl = true;
            int num_str = 0;
            Console.Clear();

            Folder_and_files.Clear();
            Folder_and_files.Add("Создать каталог.");
            Folder_and_files.Add("Создать файл.");
            Folder_and_files.Add("Удалить каталог.");
            Folder_and_files.Add("Переместить каталог.");
            if (FolderPath != (null,null)) Folder_and_files.Add("Вставить каталог.");
            if (FilePath.Item1 != null) Folder_and_files.Add("вставить файл");
            while (fl)
            {
                Console.Clear();
                Console.WriteLine("Параметры папки.");
                for (int i = 0; i < Folder_and_files.Count(); i++)
                {
                    if (i == num_str)
                    {
                        (ConsoleColor, ConsoleColor) color = GetColor();
                        Console.BackgroundColor = color.Item1;
                        Console.ForegroundColor = color.Item2;
                        Console.WriteLine(Folder_and_files[i]);
                        Console.ResetColor();
                        continue;
                    }
                    Console.WriteLine(Folder_and_files[i]);
                }
                CheckInputBtn(ref num_str, ref fl);
            }
            UpdateFolderFiles();
        }

        //обновление списка папок и файлов в каталоге.
        public void UpdateFolderFiles()
        {
            if (path == null)
            {
                Folder_and_files = DiskList();
            }
            else if (OSVersion == "Microsoft")
            {
                Folder_and_files.Clear();
                Folder_and_files.Add("...");
                DirectoryInfo[] dirs = path.GetDirectories();
                FileInfo[] files = path.GetFiles();
                foreach (var i in dirs)
                {
                    Folder_and_files.Add(i.Name);
                }
                foreach (var i in files)
                {
                    Folder_and_files.Add(i.Name);
                }
            }
            else
            {
                Folder_and_files.Clear();
                if (path.Parent != null) Folder_and_files.Add("...");
                DirectoryInfo[] dirs = path.GetDirectories();
                FileInfo[] files = path.GetFiles();
                foreach (var i in dirs)
                {
                    Folder_and_files.Add(i.Name);
                }
                foreach (var i in files)
                {
                    Folder_and_files.Add(i.Name);
                }
            }
        }
    
        //инициализация выбранного свойства.
        public void ChooseOptions(string NameOption)
        {
            if (NameOption == "Создать каталог.") CreateFolder();
            else if (NameOption == "Переместить каталог.")
            {
                if (OSVersion == "Microsoft" && path.Parent != null)
                    FolderPath = (path.FullName + "\\" + FolderName, FolderName);
                else
                    FolderPath = (path.FullName + "/" + FolderName, FolderName);

            }
            else if (NameOption == "Вставить каталог.") InsertFolder();
            else if (NameOption == "Удалить каталог.") DeleteFolder();
            else if (NameOption == "Создать файл.") WriteFile();
            else if (NameOption == "Открыть файл.") PrintFile(FileName);
            else if (NameOption == "Свойства файла.") PropertiesFiles();
            else if (NameOption == "Добавить в список объединения файлов.")
            {
                if (OSVersion == "Microsoft" && path.Parent != null)
                    MergeFiles.Add((path.FullName + "\\" + FileName));
                else
                    MergeFiles.Add(path.FullName + "/" + FileName);
            }
            else if (NameOption == "объеденить выбранные файлы") merge();
            else if (NameOption == "Копировать файл.") CopyFile();
            else if (NameOption == "Переместить файл.") MoveFile();
            else if (NameOption == "вставить файл") InsertFile();
            else if (NameOption == "Удалить файл.") DeleteFile();
        }

        public void DeleteFile()
        {
            string pt;
            if (OSVersion == "Microsoft" && path.Parent != null) pt = path.FullName + "\\" + FileName;
            else if (path.Parent != null) pt = path.FullName + "/" + FileName;
            else pt = path.FullName + FileName;
            FileInfo fileInf = new FileInfo(pt);
            if (fileInf.Exists)
            {
                fileInf.Delete();
            }
            List<int> del = new List<int>();
            if(MergeFiles.Count > 0)
            {
                for(int i = 0; i < MergeFiles.Count; i++)
                {
                    if(MergeFiles[i] == pt)
                    {
                        del.Add(i);
                    }
                }
                for(int i = 0; i < del.Count; i++)
                {
                    MergeFiles.RemoveAt(del[i]);
                }
            }
        }
        //вставить файл.
        public void InsertFile()
        {
            if (!FilePath.Item2)
            {
                FileInfo fileInf = new FileInfo(FilePath.Item1);
                if (fileInf.Exists)
                {       
                    string pt;
                    if (OSVersion == "Microsoft" && path.Parent != null)pt = path.FullName + "\\" + FileName;
                    else if(path.Parent != null)pt = path.FullName + "/" + FileName;
                    else pt = path.FullName + FileName;
                    try
                    {
                        fileInf.MoveTo(pt);
                    }
                    catch
                    {
                        Console.WriteLine("Возникла не предвиденная ошибка при перемещении файла, попробуйте снова.");
                        Console.WriteLine("Для продолжения нажмите на кнопку.");
                        var a = Console.ReadKey();
                        if (a.Key == ConsoleKey.H) Help();
                    }
                }
                else Console.WriteLine("Файл удален.");
            }
            else
            {
                FileInfo fileInf = new FileInfo(FilePath.Item1);
                if (fileInf.Exists)
                {
                    string pt;
                    if (OSVersion == "Microsoft" && path.Parent != null) pt = path.FullName + "\\" + FileName;
                    else if (path.Parent != null) pt = path.FullName + "/" + FileName;
                    else pt = path.FullName + FileName;
                    try
                    {
                        fileInf.CopyTo(pt);
                    }
                    catch
                    {
                        Console.WriteLine("Возникла не предвиденная ошибка при копировании файла, попробуйте снова.");
                        Console.WriteLine("Для продолжения нажмите на кнопку.");
                        var a = Console.ReadKey();
                        if (a.Key == ConsoleKey.H) Help();
                    }
                }
                else Console.WriteLine("Файл удален.");          
            }
            FilePath.Item1 = null;
        }

        //запомнить файл, и поставить метку перемещения.
        public void MoveFile()
        {
            string pt;
            FilePath = (path.FullName + FileName, false);
            if (OSVersion == "Microsoft" && path.Parent != null)
                FilePath = (path.FullName + "\\" + FileName, false);
            else if(path.Parent != null)
                FilePath = (path.FullName + "/" + FileName, false);
            
        }

        //запомнить файл, и поставить метку копирования.
        public void CopyFile()
        {
            string pt;
            FilePath = (path.FullName + FileName, true);
            if (OSVersion == "Microsoft" && path.Parent != null)
                FilePath = (path.FullName + "\\" + FileName, true);
            else if (path.Parent != null)
                FilePath = (path.FullName + "/" + FileName, true);
        }

        //объединение файлов.
        public void merge()
        {
            string NameFile;
            NameFile = CreateFiles();
            System.Text.Encoding enc = OpenFiles();
            List<string> text = new List<string>();
            foreach (string pt in MergeFiles)
            {
                using (StreamReader sr = new StreamReader(pt))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        text.Add(line);
                    }
                }
                using (StreamWriter sw = new StreamWriter(NameFile, true, enc))
                {
                    foreach (string line in text)
                    {
                        sw.WriteLine(line);
                    }
                    text.Clear();
                }
            }
        }

        //вывод свойства файлов.
        public void PropertiesFiles()
        {
            Console.Clear();
            FileInfo fileInf = new FileInfo(path + (OSVersion == "Microsoft" ? "\\" : "/") + FileName);
            Console.WriteLine("Имя файла: {0}.", fileInf.Name);
            Console.WriteLine("Время создания: {0}.", fileInf.CreationTime);
            Console.WriteLine("Размер: {0}.", fileInf.Length);
            Console.WriteLine("Последняя запись: {0}.", fileInf.LastWriteTime);
            Console.WriteLine("Для продолжения нажмите любую кнопку.");
            var a = Console.ReadKey();
            if (a.Key == ConsoleKey.H) Help();

        }

        //перемещение папки.
        public void InsertFolder()
        {
            string pat;
            if (OSVersion == "Microsoft" && path.Parent != null)
                pat = path.FullName + "\\" + FolderPath.Item2;
            else
                pat = path.FullName + "/" + FolderPath.Item2;

            if (Directory.Exists(pat))
            {
                Console.WriteLine("Папка с данным названием существует, ввыберите другой каталог, или удалите каталог с таким же названием.");
            }
            else
            {
                Directory.Move(FolderPath.Item1, pat);
            }
            UpdateFolderFiles();
            FolderPath = (null, null);
        }
    
        //создание папки.
        public void CreateFolder()
        {
            Console.WriteLine("Введите название папки.");
            while (true)
            {
                string name = Console.ReadLine();
                while (true)
                {
                    if(name != "") break;
                    Console.WriteLine("Пустая строка");
                    name = Console.ReadLine();
                }
                if (OSVersion == "Microsoft" && path != null && path.Parent != null)
                    name = path.FullName + "\\" + name;
                else if (path != null && path.Parent != null)
                    name = path.FullName + "/" + name;
                else if(path != null)
                    name = path.FullName + name;
                if (Directory.Exists(name))
                {
                    Console.WriteLine("Папка с данным названием существует, введите другое название.");
                    continue;
                }
                else
                {
                    Directory.CreateDirectory(name);
                    Console.WriteLine("Папка успешно создана");
                    Console.WriteLine("Для выхода в директории нажмите любую кнопку. Для помощи нажмите H.");
                    var a = Console.ReadKey();
                    if (a.Key == ConsoleKey.H) Help();
                    break;
                }
            }

        }
        
        //удаление папки.
        public void DeleteFolder(string nm = "", DirectoryInfo pt = null)
        {
            bool fl = true;
            string name = "";
            if (nm == "") nm = FolderName;
            if (pt == null) pt = path; 
            if (OSVersion == "Microsoft" && pt != null && pt.Parent != null)
                name = pt.FullName + "\\" + nm;
            else if (pt != null && pt.Parent != null)
                name = path.FullName + "/" + nm;
            else if (pt != null)
                name = pt.FullName + nm;
            else
            {
                Console.WriteLine("Ошибка при удалении каталога.");
                Console.WriteLine("Для продолжение нажмите на любую клавишу");
                Console.ReadKey();
                fl = false;
            }
            if (fl)
            {
                DirectoryInfo[] dirs = new DirectoryInfo(name).GetDirectories();
                FileInfo[] fi = new DirectoryInfo(name).GetFiles();
                foreach (DirectoryInfo df in dirs)
                {
                    try
                    {
                        DeleteFolder(df.Name,new DirectoryInfo(name));
                        
                    }
                    catch (System.IO.IOException ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                var a = new DirectoryInfo(name);
                try
                {
                    a.Delete(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            
        }
    
        //создание файла.
        public string CreateFiles()
        {
            
            while (true)
            {   string name = Console.ReadLine();
                Console.WriteLine("Введите название файла с расширением.");
                while (true)
                {
                    if(name != "")
                    {
                        break;
                    }
                    Console.WriteLine("Пустая строка");
                    name = Console.ReadLine();
                }
                if(name.Split('.').Length < 2 && name.Split('.')[1] == "")
                {
                    Console.WriteLine("Некорректное название файла.");
                    continue;
                }
                if (OSVersion == "Microsoft" && path != null && path.Parent != null)
                    name = path.FullName + "\\" + name;
                else if (path != null && path.Parent != null)
                    name = path.FullName + "/" + name;
                else name = path.FullName + name;
                FileInfo fl = new FileInfo(name);
                if (fl.Exists)
                {
                    Console.WriteLine("Файл с таким именем существует. Хотите перезаписать данный файл?  Y/N");
                    var Keypress = Console.ReadKey();
                    if (Keypress.Key == ConsoleKey.H) Help();
                    while (Keypress.Key != ConsoleKey.Y && Keypress.Key != ConsoleKey.N)
                    {
                        Console.WriteLine("Пожалуйста повторите попытку");
                        Keypress = Console.ReadKey();
                        if (Keypress.Key == ConsoleKey.H) Help();
                    }
                    
                    if(Keypress.Key == ConsoleKey.N)
                    {
                        continue;
                    }
                }
                try
                {
                    using (FileStream fs = fl.Create())
                    {
                    }
                    return name;
                }
                catch
                {
                    Console.WriteLine("Не удалось создать файл");
                    Console.WriteLine("для продолжения нажмите на любую кнопку");
                    var a = Console.ReadKey();
                    if (a.Key == ConsoleKey.H) Help();
                    return "error";
                }
            }
        }

        //запись в файл.
        public void WriteFile()
        {
            string name = CreateFiles();
            if (name != "error")
            {
                var enc = OpenFiles();
                Console.WriteLine("Заполните файл");
                do
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(name, false, enc))
                        {
                            sw.WriteLine(Console.ReadLine());
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Извините формат файла не поддерживается для записи.");
                        return;
                    }
                    Console.WriteLine("Продолжить? Y/N");

                    while (true)
                    {
                        var key = Console.ReadKey();
                        if (key.Key == ConsoleKey.N) return;
                        else if (key.Key == ConsoleKey.Y) break;
                        else if (key.Key == ConsoleKey.H) Help();
                        else Console.WriteLine("Пожалуйста повторите попытку.");
                    }
                } while (true);
            }
            
        }
    
    }

    

    class Program
    {
        static void Main(string[] args)
        {
            FileManagers start = new FileManagers();
            start.start();
        }
    }
}
