"""Save the exact packed pixels in editable RGBA Aseprite timelines.
Spec: https://github.com/aseprite/aseprite/blob/main/docs/ase-file-specs.md
No image content changes. Native CLI alternative: salon-patrons-source.lua.
"""
from pathlib import Path
from PIL import Image
import struct as st,zlib,sys

folder=Path(sys.argv[1]);atlas=Image.open(folder/'FrequentadoresSinuca.png').convert('RGBA')
def chunk(kind,data):return st.pack('<IH',len(data)+6,kind)+data
def string(value):
    data=value.encode('utf-8');return st.pack('<H',len(data))+data
tags=[(0,0,'frente_repouso'),(1,2,'frente_conversa'),(3,3,'frente_bebendo'),(4,4,'perfil_repouso'),(5,5,'perfil_conversa'),(6,6,'perfil_bebendo'),(7,7,'costas_repouso')]
for row,name in enumerate(('Frequentador','Chico','Ramiro','Motorista')):
    frames=[];raw_cells=[]
    for pose in range(8):
        chunks=[]
        if pose==0:
            chunks.append(chunk(0x2004,st.pack('<6HB3x',3,0,0,128,128,0,255)+string(name)))
            data=st.pack('<H8x',len(tags))
            for start,end,label in tags:data+=st.pack('<HHBH6x3BB',start,end,0,0,179,145,85,0)+string(label)
            chunks.append(chunk(0x2018,data))
        raw=atlas.crop((pose*128,row*128,(pose+1)*128,(row+1)*128)).tobytes();raw_cells.append(raw)
        chunks.append(chunk(0x2005,st.pack('<HhhBHh5xHH',0,0,0,255,2,0,128,128)+zlib.compress(raw)))
        body=b''.join(chunks)
        frames.append(st.pack('<IHHH2xI',16+len(body),0xF1FA,len(chunks),240,len(chunks))+body)
    body=b''.join(frames);header=bytearray(128)
    st.pack_into('<I5HIH',header,0,128+len(body),0xA5E0,8,128,128,32,1,240)
    header[34:36]=bytes((1,1))
    output=folder/(name+'.aseprite');output.write_bytes(header+body)
    # Independent chunk traversal validates every compressed cel against the atlas.
    data=output.read_bytes();offset=128;found=[];tag_count=0
    while offset<len(data):
        size,magic,count=st.unpack_from('<IHH',data,offset);assert magic==0xF1FA
        cursor=offset+16
        for _ in range(count):
            length,kind=st.unpack_from('<IH',data,cursor)
            if kind==0x2005:found.append(zlib.decompress(data[cursor+26:cursor+length]))
            if kind==0x2018:tag_count=st.unpack_from('<H',data,cursor+6)[0]
            cursor+=length
        assert cursor==offset+size;offset+=size
    assert found==raw_cells and tag_count==7 and offset==len(data)
    print(f'PASS: {output.name}: 8 RGBA cels, 7 pose tags, pixel-exact round trip')
