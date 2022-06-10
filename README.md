# KNFE (K.N.'s File Extractor)
KNFE (pronounced "knife") is a C#-based GUI & CLI program for extracting and decoding various file formats.

The current focus for KNFE falls upon the following types of file formats:
* File formats without modern implementations
* Proprietary video game file formats
* Generally old & obscure file formats

## Usage
```console
KNFE.CLI -i <infile> -f <formatcode> [-o <outdir>] [-v]
```
Or, double-click KNFE.UI to use the GUI version.

## Supported Formats
| File Format / Encoding  | Format Code |
| ----------------------- | ----------- |
| Fallout 1 DAT (*.DAT)   | ``fallout`` |

## Background
This project started as a passion project several years back while I was thumbing through the [UTZOO Wiseman Usenet Archive](https://archive.org/details/utzoo-wiseman-usenet-archive) and discovering lots of file & encoding formats I was unfamiliar with. For a while, I had already been working to reverse engineer various video game file formats, and so I took interest in creating modern implementations of the utilities that decoded these obscure Usenet files.

An earlier version of KNFE exists, but it was poorly structured and not designed for expandability. Thus, this completely re-written and re-factored project was born.

## Roadmap
- [X] Recreate KNFE's core functions as a library (KNFE.Core.dll)
- [X] Basic UI frontend
- [X] Basic CLI
- [ ] File format identification based on file data (eventually remove FSF, etc.)
- [ ] Expand EncodingStreams and derived classes to derive from Streams (replicate GZipStream behaviour)
- [ ] Implement icons for UI tree
- [ ] Unify EncodingStreams for related encodings (LZSS variants, preset class)

## License
This project is licensed under the [GNU GPLv3](LICENSE).