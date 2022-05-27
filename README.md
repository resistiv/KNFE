# v2 is currently under construction.
## Roadmap
- [X] Recreate KNFE's core functions as a library (KNFE.Core.dll)
- [X] Basic UI frontend
- [ ] Basic CLI
- [ ] Decide on a license (GNU GPLv3?)
- [ ] File format identification based on file data (eventually remove FSF) (optional for v2)
- [ ] Expand EncodingStreams and derived classes to derive from Streams (replicate GZipStream behaviour) (optional for v2)
- [ ] Implement icons for UI tree (optional for v2)
- [ ] Unify EncodingStreams for related encodings (LZSS variants, preset class) (optional for v2)

# KNFE (K.N.'s File Extractor)
KNFE (pronounced "knife") is a C#-based GUI & CLI program for extracting and decoding various file formats.

The current focus for KNFE falls upon the following types of file formats:
* File formats without modern implementations
* Proprietary video game file formats
* Generally old & obscure file formats

## Background
This project started as a passion project several years back while I was thumbing through the [UTZOO Wiseman Usenet Archive](https://archive.org/details/utzoo-wiseman-usenet-archive) and discovering lots of file & encoding formats I was unfamiliar with. For a while, I had already been working to reverse engineer various video game file formats, and so I took interest in creating modern implementations of the utilities that decoded these obscure Usenet files.

An earlier version of KNFE exists, but it was poorly structured and not designed for expandability. Thus, this completely re-written and re-factored project was born.

## Supported Formats

| File Format / Encoding  | Short Code | Long Code       |
| ----------------------- | ---------- | --------------- |
| BinHex 4.0 (*.hqx)      | ``bh4``    | ``binhex4.0``   |
| Fallout 1's DAT (*.DAT) | ``f1d``    | ``fallout1dat`` |
| uuencode (*.uu, *.uue)  | ``uue``    | ``uuencode``    |

## Usage
```console
knfe -i [file path] -t [short code | long code]
```

## License