using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HumanAPIClient.Enum;
using HumanAPIClient.Util.Exceptions;

namespace HumanAPIClient.Model
{
    /// <summary>
    /// Classe responsavel por armazenar um arquivo com seu tipo de layout para ser enviada 
    /// para o gateway de sms.
    /// </summary>
    public class FileResource : MultipleMessage
    {
        /// <summary>
        /// Arquivo que armazena o conteudo
        /// </summary>
        private FileInfo content;
    	
        /// <summary>
        /// Construtor da classe onde eh informado o arquivo e o tipo de layout
        /// </summary>
        /// <param name="file"></param>
        /// <param name="type"></param>
        public FileResource(FileInfo file, LayoutTypeEnum type) 
            : base(type) 
        {
		    content = file;
	    }
    	
        /// <summary>
        /// Retorna o arquivo amazenado
        /// </summary>
        /// <returns></returns>
        public FileInfo getFile()
        {
		    return content;
	    }
    	
	    override public String Content
        {
            get 
            {
		        StringBuilder result = new StringBuilder();
        		
		        //Valida o tamanho do arquivo
		        if (content.Length > (1024 * 1024)) {
			        throw new ClientHumanException("File size exceeds the limit of 1MB.");
		        }

                try
                {
                    FileStream fs = content.OpenRead();
                    StreamReader sr = new StreamReader(fs);
                    String line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        result.AppendLine(line);
                    }

                    fs.Close();
                }
                catch (UnauthorizedAccessException e)
                {
                    throw new ClientHumanException("File not readable.");
                }
                catch (DirectoryNotFoundException e)
                {
                    throw new ClientHumanException("File not found.");
                }
                catch (IOException e)
                {
                    throw new ClientHumanException("File not readable.");
                }
        		
		        return result.ToString();
            }
	    }
    }
}
