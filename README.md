# KNFE (K.N.'s File Extractor)
KNFE (pronounced "knife") is a C#-based GUI & CLI program for extracting and decoding various file formats.

The current focus for KNFE falls upon the following types of file formats:
* File formats without modern implementations
* Undocumented video game file formats
* Generally old & obscure file formats

## Usage
```console
KNFE.CLI -i <infile> -f <formatcode> [-o <outdir>] [-v]
```
Or, double-click KNFE.UI to use the GUI version.

## Supported Formats
| File Format / Encoding  | Extension(s) | Format Code |
| ----------------------- | ------------ | ----------- |
| BinHex 4.0              | ``hqx``      | ``binhex4`` |
| Fallout 1 DAT           | ``dat``      | ``fallout`` |
| Uuencode            | ``uu``, ``uue``  | ``uuencode``|

## Background
This project started as a passion project several years back while I was thumbing through the [UTZOO Wiseman Usenet Archive](https://archive.org/details/utzoo-wiseman-usenet-archive) and discovering lots of file & encoding formats I was unfamiliar with. For a while, I had already been working to reverse engineer various video game file formats, and so I took interest in creating modern implementations of the utilities that decoded these obscure Usenet files.

An earlier version of KNFE exists, but it was poorly structured and not designed for expandability. Thus, this completely re-written and re-factored project was born.

## Roadmap
- [X] Recreate KNFE's core functions as a library (KNFE.Core.dll)
- [X] Basic UI frontend
- [X] Basic CLI
- [X] Create KNFE.Test for testing output, efficiency, etc.
- [X] Add detailed breakdowns of formats with sources, link in Supported Formats table
- [X] Implement icons for UI tree
- [ ] File format identification based on file data (eventually remove FSF, etc.)
- [ ] Expand EncodingStreams and derived classes to derive from Streams (replicate GZipStream behaviour)
- [ ] Unify EncodingStreams for related encodings (LZSS variants, preset class)

## License
This project is licensed under the [GNU GPLv3](LICENSE).
