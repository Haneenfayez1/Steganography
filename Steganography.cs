using System;
using System.Drawing;
using System.Text;

class Steganography
{
    public static void EncodeText(string inputImagePath, string outputImagePath, string message)
    {
        Bitmap bitmap = new Bitmap(inputImagePath);
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        byte[] lengthBytes = BitConverter.GetBytes(messageBytes.Length);
        byte[] fullMessage = new byte[lengthBytes.Length + messageBytes.Length];
        Array.Copy(lengthBytes, fullMessage, lengthBytes.Length);
        Array.Copy(messageBytes, 0, fullMessage, lengthBytes.Length, messageBytes.Length);
        
        int index = 0;
        for (int i = 0; i < bitmap.Height; i++)
        {
            for (int j = 0; j < bitmap.Width; j++)
            {
                if (index >= fullMessage.Length) break;
                Color pixel = bitmap.GetPixel(j, i);
                byte r = (byte)((pixel.R & 0xFE) | ((fullMessage[index] >> 7) & 1));
                byte g = (byte)((pixel.G & 0xFE) | ((fullMessage[index] >> 6) & 1));
                byte b = (byte)((pixel.B & 0xFE) | ((fullMessage[index] >> 5) & 1));
                bitmap.SetPixel(j, i, Color.FromArgb(r, g, b));
                index++;
            }
        }
        bitmap.Save(outputImagePath);
        Console.WriteLine("Message encoded successfully!");
    }
    
    public static string DecodeText(string imagePath)
    {
        Bitmap bitmap = new Bitmap(imagePath);
        byte[] lengthBytes = new byte[4];
        int index = 0;
        
        for (int i = 0; i < bitmap.Height; i++)
        {
            for (int j = 0; j < bitmap.Width; j++)
            {
                if (index >= lengthBytes.Length) break;
                Color pixel = bitmap.GetPixel(j, i);
                lengthBytes[index] = (byte)((pixel.R & 1) << 7 | (pixel.G & 1) << 6 | (pixel.B & 1) << 5);
                index++;
            }
        }
        
        int messageLength = BitConverter.ToInt32(lengthBytes, 0);
        byte[] messageBytes = new byte[messageLength];
        index = 0;
        
        for (int i = 0; i < bitmap.Height; i++)
        {
            for (int j = 0; j < bitmap.Width; j++)
            {
                if (index >= messageLength) break;
                Color pixel = bitmap.GetPixel(j, i);
                messageBytes[index] = (byte)((pixel.R & 1) << 7 | (pixel.G & 1) << 6 | (pixel.B & 1) << 5);
                index++;
            }
        }
        
        return Encoding.UTF8.GetString(messageBytes);
    }
    
    public static void Main()
    {
        Console.WriteLine("Choose an option: \n1. Encode Message\n2. Decode Message");
        int choice = int.Parse(Console.ReadLine());
        
        if (choice == 1)
        {
            Console.Write("Enter input image path: ");
            string inputImagePath = Console.ReadLine();
            Console.Write("Enter output image path: ");
            string outputImagePath = Console.ReadLine();
            Console.Write("Enter message to hide: ");
            string message = Console.ReadLine();
            EncodeText(inputImagePath, outputImagePath, message);
        }
        else if (choice == 2)
        {
            Console.Write("Enter encoded image path: ");
            string imagePath = Console.ReadLine();
            string decodedMessage = DecodeText(imagePath);
            Console.WriteLine("Decoded message: " + decodedMessage);
        }
        else
        {
            Console.WriteLine("Invalid option.");
        }
    }
}
