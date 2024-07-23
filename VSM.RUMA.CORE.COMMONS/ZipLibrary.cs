using System;
using System.IO;
using System.Collections.Generic;
using java.util;
using java.util.zip;
using java.io;
using System.IO.Compression;

namespace VSM.RUMA.CORE
{
    public class ZipLibrary
    {
        /*
         Do not use Copy local=true with vjslib, Install J# Redistributable instead
         */
        [Obsolete ("Use another Library to Zip: not vjslib")]
        public static void CreateZipFile(string DestinationZipFileName, List<String> SourceFilesToZip)
        {
            FileOutputStream fileOutStream = new FileOutputStream(DestinationZipFileName);
            ZipOutputStream zipOutStream = new ZipOutputStream(fileOutStream);
            FileInputStream fileInStream;
            ZipEntry zipEntry;
            foreach (String SourceFileToZip in SourceFilesToZip)
            {
                fileInStream = new FileInputStream(SourceFileToZip);
                zipEntry = new ZipEntry(Path.GetFileName(SourceFileToZip));
                zipOutStream.putNextEntry(zipEntry);
                sbyte[] buffer = new sbyte[1024];
                int len = 0;
                while ((len = fileInStream.read(buffer)) >= 0)
                {
                    zipOutStream.write(buffer, 0, len);
                }
                zipOutStream.closeEntry();
                fileInStream.close();
            }
            zipOutStream.close();
            fileOutStream.close();
        }

        [Obsolete("Use another Library to Zip: not vjslib")]
        public static void CreateZipFile(string DestinationZipFileName, string SourceFileToZip)
        {
            FileOutputStream fileOutStream = new FileOutputStream(DestinationZipFileName);
            ZipOutputStream zipOutStream = new ZipOutputStream(fileOutStream);
            FileInputStream fileInStream = new FileInputStream(SourceFileToZip);
            ZipEntry zipEntry = new ZipEntry(Path.GetFileName(SourceFileToZip));
            zipOutStream.putNextEntry(zipEntry);
            sbyte[] buffer = new sbyte[1024];
            int len = 0;
            while ((len = fileInStream.read(buffer)) >= 0)
            {
                zipOutStream.write(buffer, 0, len);
            }
            zipOutStream.closeEntry();
            fileInStream.close();
            zipOutStream.close();
            fileOutStream.close();
        }

        [Obsolete("Use another Library to Zip: not vjslib")]
        public static int ExtractZipFile(string SourceZipFileName, string DestinationPath)
        {
            int result = 0;
            FileInputStream fileInStream = new FileInputStream(SourceZipFileName);
            ZipInputStream zipInStream = new ZipInputStream(fileInStream);
            ZipEntry zipEntry = zipInStream.getNextEntry();
            while (zipEntry != null && zipEntry.getName() != String.Empty)
            {
                if (zipEntry.isDirectory())
                {
                    Directory.CreateDirectory(DestinationPath + zipEntry.getName());
                }
                else
                {
                    FileOutputStream fileOutStream = new FileOutputStream(DestinationPath + zipEntry.getName());
                    zipEntry.getSize();
                    sbyte[] buffer = new sbyte[1024];
                    int len = 0;
                    while ((len = zipInStream.read(buffer)) >= 0)
                    {
                        fileOutStream.write(buffer, 0, len);
                    }
                    fileOutStream.close();
                    result++;
                }
                zipEntry = zipInStream.getNextEntry();
            }
            zipInStream.close();
            fileInStream.close();
            return result;
        }


        [Obsolete("Use another Library to Zip: not vjslib")]
        public static bool ExtractSingleFile(string SourceZipFileName, String ZippedFileName, String DestinationFileName)
        {
            FileInputStream fileInStream = new FileInputStream(SourceZipFileName);
            ZipInputStream zipInStream = new ZipInputStream(fileInStream);
            ZipEntry zipEntry = zipInStream.getNextEntry();
            while (zipEntry != null && zipEntry.getName() != String.Empty)
            {
                if (!zipEntry.isDirectory() && zipEntry.getName() == ZippedFileName)
                {
                    FileOutputStream fileOutStream = new FileOutputStream(DestinationFileName);
                    zipEntry.getSize();
                    sbyte[] buffer = new sbyte[1024];
                    int len = 0;
                    while ((len = zipInStream.read(buffer)) >= 0)
                    {
                        fileOutStream.write(buffer, 0, len);
                    }
                    fileOutStream.close();
                }
                zipEntry = zipInStream.getNextEntry();
            }
            zipInStream.close();
            fileInStream.close();
            return false;
        }

        [Obsolete("Use another Library to Zip: not vjslib")]
        public static System.Collections.Generic.List<String> FilesInZipFile(string SourceZipFileName)
        {
            System.Collections.Generic.List<String> Result = new System.Collections.Generic.List<String>();
            FileInputStream fileInStream = new FileInputStream(SourceZipFileName);
            ZipInputStream zipInStream = new ZipInputStream(fileInStream);
            ZipEntry zipEntry = zipInStream.getNextEntry();
            while (zipEntry != null && zipEntry.getName() != String.Empty)
            {
                if (!zipEntry.isDirectory())
                {
                    Result.Add(zipEntry.getName());
                }
                zipEntry = zipInStream.getNextEntry();
            }
            zipInStream.close();
            fileInStream.close();
            return Result;
        }

        [Obsolete("Use another Library to Zip: not vjslib")]
        public static void UncompressFile(string SourceZipFileName, String ZippedFileName, String DestinationFileName)
        {
            using (FileStream sourceFile = System.IO.File.OpenRead(SourceZipFileName))
            {
                using (FileStream destinationFile = System.IO.File.Create(DestinationFileName))
                {

                    // Because the uncompressed size of the file is unknown, 
                    // we are using an arbitrary buffer size.
                    byte[] buffer = new byte[4096];
                    int n;

                    using (GZipStream input = new GZipStream(sourceFile,
                        CompressionMode.Decompress, false))
                    {
                        Console.WriteLine("Decompressing {0} to {1}.", sourceFile.Name,
                            destinationFile.Name);

                        n = input.Read(buffer, 0, buffer.Length);
                        destinationFile.Write(buffer, 0, n);
                    }
                }
            }
        }

    }
}