using System.Buffers.Binary;
using System.IO;

namespace KNFE.Core.Encoding.Compression
{
    /// <summary>
    /// Handles Fallout 1's LZSS compression algorithm.
    /// </summary>
    public class Fallout1LzssStream : EncodingStream
    {
        // Private members
        private readonly BinaryReader _br;
        private readonly long _streamEnd;
        private byte[] _dict;
        private short _numBytes;
        private short _bytesRead;
        private short _dictOfs;
        private short _dictIndex;
        private short _matchLen;

        // Constants
        private const int DICT_SIZE = 4096;
        private const int MIN_MATCH = 3;
        private const int MAX_MATCH = 18;

        /// <summary>
        /// Initializes a new instance of a <see cref="Fallout1LzssStream"/> class with a specified source <see cref="Stream"/> and stream length.
        /// </summary>
        /// <param name="stream">The source <see cref="Stream"/> to read from.</param>
        /// <param name="streamLength">The length, in bytes, of raw data to read from the source <see cref="Stream"/>.</param>
        public Fallout1LzssStream(Stream stream, long streamLength)
            : base(stream)
        {
            _br = new BinaryReader(Stream);
            _streamEnd = streamLength + Stream.Position;
            _dict = new byte[DICT_SIZE];
        }

        public override void Decode(Stream outStream)
        {
            // Adapted from the pseudocode implementation of Shadowbird's algorithm
            // https://falloutmods.fandom.com/wiki/DAT_file_format#Fallout_1_LZSS_uncompression_algorithm
            // https://www.nma-fallout.com/threads/fallout-dat-files.160366/

            // Referenced Mr.Stalin's DAT Explorer II for bug-fixing
            // GitHub repo is outdated, used dotPeek on updated DatLab DLL
            // https://www.nma-fallout.com/resources/fallout-dat-explorer-ii.121/

            // While I did not reference this code, here's an extra implementation utilized by the FIFE engine
            // https://github.com/fifengine/fifengine/blob/master/engine/core/vfs/dat/lzssdecoder.cpp

            // Of importantance is that this stream is big endian, and thus needs conversion to little endian for number values larger than a byte
            // Literal runs of bytes do not need to be converted, as they are literal

            while (!IsLastByte())
            {
                // Number of bytes we'll read from the next block of data
                _numBytes = BinaryPrimitives.ReverseEndianness(_br.ReadInt16());

                // End of LZSS stream
                if (_numBytes == 0)
                    break;
                else
                {
                    _bytesRead = 0;

                    // If the numBytes is negative, we take the absolute value of numBytes and read that many literal bytes
                    if (_numBytes < 0)
                    {
                        _numBytes *= -1;

                        // Some files attempt to overread the boundary of their lengths; if we find such a case,
                        // trim the literal byte read length to however many bytes remain
                        if (_numBytes + base.Stream.Position > _streamEnd)
                            _numBytes = (short)(_streamEnd - base.Stream.Position);

                        byte[] tempData = _br.ReadBytes(_numBytes);
                        outStream.Write(tempData, 0, tempData.Length);

                        continue;
                    }
                    // Else, our read will be LZSS encoded

                    // For every new block of compressed data, we flush the dictionary in order to read in new values
                    ClearDictionary();
                    while (_bytesRead < _numBytes && !IsLastByte())
                    {
                        // Get a flag byte
                        byte flags = _br.ReadByte();
                        _bytesRead++;

                        // Check for premature stream end; occurs with random padding
                        if (_bytesRead >= _numBytes || IsLastByte())
                            break;

                        // Iterate through each bit of the flags byte
                        for (int i = 0; i < 8; i++)
                        {
                            // If bit is set, we have a literal byte; write to output and to dictionary
                            if (flags % 2 != 0)
                            {
                                byte b = _br.ReadByte();
                                _bytesRead++;
                                outStream.WriteByte(b);
                                WriteToDictionary(b);

                                // Premature end check
                                if (_bytesRead >= _numBytes || IsLastByte())
                                    break;
                            }
                            // If bit is NOT set, we have an encoded run, reference dictionary
                            else
                            {
                                // Premature end check
                                if (_bytesRead >= _numBytes || IsLastByte())
                                    break;

                                // Read the offset of our encoded data in the dictionary
                                _dictOfs = _br.ReadByte();
                                _bytesRead++;

                                // Premature end check
                                if (_bytesRead >= _numBytes || IsLastByte())
                                    break;

                                // Read how long the match is in bytes
                                _matchLen = _br.ReadByte();
                                _bytesRead++;

                                // Prepend the 4 MSBs of matchLen to dOfs to get the full dictionary offset;
                                // 8 bits can only hold up to 255, while 12 bits can hold up to 4095, or our dictionary length
                                _dictOfs |= (short)((_matchLen & 0xF0) << 4);
                                // Discard the 4 MSBs of matchLen since we added them to dOfs; matchLen can be up to 15 bytes
                                _matchLen &= 0x0F;
                                // Then, read from the dictionary to output
                                for (int j = 0; j < _matchLen + MIN_MATCH; j++)
                                {
                                    byte b = ReadDictionary();
                                    outStream.WriteByte(b);
                                    WriteToDictionary(b);
                                }
                            }

                            // Get next flag
                            flags >>= 1;

                            // Premature end check
                            if (IsLastByte())
                                break;
                        }
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Flushes the dictionary and resets the dictionary index.
        /// </summary>
        private void ClearDictionary()
        {
            // Fills the dictionary with ' ' chars to overwrite residual data
            for (int i = 0; i < DICT_SIZE; i++)
                _dict[i] = 0x20;
            
            // Reset our dictionary index
            _dictIndex = DICT_SIZE - MAX_MATCH;
        }

        /// <summary>
        /// Reads from the LZSS dictionary and advances the current dictionary offset.
        /// </summary>
        /// <returns>A byte read from the dictionary.</returns>
        private byte ReadDictionary()
        {
            byte b = _dict[_dictOfs++ % DICT_SIZE];
            // If we overrun the dictionary, loop to beginning
            _dictOfs %= DICT_SIZE;
            return b;
        }

        /// <summary>
        /// Writes to the LZSS dictionary and advances the dictionary write index.
        /// </summary>
        /// <param name="b">A <see cref="byte"/>byte to write to the dictionary.</param>
        private void WriteToDictionary(byte b)
        {
            _dict[_dictIndex++ % DICT_SIZE] = b;
            // If we overrun the dictionary, loop to beginning
            _dictIndex %= DICT_SIZE;
        }

        /// <summary>
        /// Checks if the end of the base <see cref="Stream"/> has been reached.
        /// </summary>
        /// <returns><c>true</c> if the end of the base <see cref="Stream"/> has been reached; <c>false</c> otherwise.</returns>
        private bool IsLastByte() => _br.BaseStream.Position == _streamEnd;
    }
}
