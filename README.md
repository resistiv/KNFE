# KNFE (K.N.'s File Extractor)
KNFE (pronounced "knife") is a C#-based CLI program for extracting and decoding various file formats.

The current focus for KNFE falls upon the following types of file formats:
* File formats without modern implementation
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

## Prequisites
This project makes use of silkfire's <a href="https://github.com/silkfire/Pastel">Pastel</a> package.
```console
nuget install Pastel -Version 2.1.0
```

## License
This project is licensed under the [MIT License](LICENSE).
