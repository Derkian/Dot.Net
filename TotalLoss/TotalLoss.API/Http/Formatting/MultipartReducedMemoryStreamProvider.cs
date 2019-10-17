using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace TotalLoss.API.Http.Formatting
{
    public class MultipartReducedMemoryStreamProvider : MultipartStreamProvider
    {
        Func<MemoryStream> _memoryStreamGetter;

        /// <summary>
        /// MultipartReducedMemoryStreamProvider
        /// </summary>
        /// <param name="memoryStreamGetter">User provided factory used for creating a memory stream to use for file upload parts</param>
        public MultipartReducedMemoryStreamProvider(Func<MemoryStream> memoryStreamGetter)
        {
            _memoryStreamGetter = memoryStreamGetter;
        }

        /// <summary>Gets the stream where to write the body part to. This method is called when a MIME multipart body part has been parsed.</summary>
        /// <returns>The <see cref="Stream" /> instance where the message body part is written to.</returns>
        /// <param name="parent">The content of the HTTP.</param>
        /// <param name="headers">The header fields describing the body part.</param>
        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (headers == null)
                throw new ArgumentNullException(nameof(headers));

            Stream result = null;
            if (SuggestsFileContent(headers) && _memoryStreamGetter != null)
            {
                result = _memoryStreamGetter();
            }
            else
            {
                try
                {
                    result = new MemoryStream(headers.ContentLength.HasValue ? Convert.ToInt32(headers.ContentLength.Value) : 1024 * 1024);
                }
                catch (ObjectDisposedException)
                {
                    result = new MemoryStream();
                }
            }

            return result;
        }

        public static bool SuggestsFileContent(HttpContentHeaders headers)
        {
            return !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName)
                            || headers.ContentDisposition.Name?.ToLowerInvariant() == "file"
                            || (headers.ContentDisposition.DispositionType?.ToLowerInvariant()?.StartsWith("image") ?? false);
        }

    }
}