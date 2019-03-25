Steins Gate Linear Bounded Phenogram Image converter (Raw->PNG and PNG-->Raw). PNGs MUST BE 8 BIT IMAGES WITH INDEXED 32 BIT COLORS

# Images File Structure

## Header
##### Total 8 bytes
The header of the file is

Name | Size |
--- | --- 
Width | 2 bytes 
Height | 2 bytes 
Probably, index size (Always 8) | 4 bytes 


## Color Map
##### Total 1024 bytes (4 Bytes per color)

Name | Size |
--- | --- 
Color n | 4 bytes

Repeat this 256 times

## Raw data
##### Total width*height bytes

Name | Size |
--- | --- 
Pixel n | 1 byte
