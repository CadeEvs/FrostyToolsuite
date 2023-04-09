### ModSupport
- [ ] Move fbmod related stuff into ModSupport
- [ ] Add CasWriter which writes data directly to cas so we dont have to store it in memory

### Sdk
- [ ] Add TypeLibrary with lookups for name and hash
- [ ] Move game sdk creation into Sdk
- [ ] Use Microsoft.CodeAnalysis to generate code for classes
- [ ] Add RiffEbxReader and all EbxWriters and maybe move them into static (De)Serialize methods
- [ ] Make CasStream into static method that decompresses the incoming stream
