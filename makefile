gamecloud-proto:
	protoc \
  --csharp_out=GameCloud.Unity/Runtime/Scripts/Generated \
  --csharp_opt=file_extension=.g.cs \
  --proto_path=proto \
  proto/rtapi.proto \
  proto/gamecloud.proto


