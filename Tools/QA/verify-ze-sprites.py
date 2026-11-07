"""Asset checks for the newly generated ZÃƒÂ© sheet. Does not assert artistic approval."""
from pathlib import Path
from PIL import Image
import hashlib
root=Path(__file__).resolve().parents[2]
p=root/'Projeto Jogo 2D/Assets/Resources/Art/Characters/Npc_Ze_Sinuca.png'
im=Image.open(p)
assert im.mode=='RGBA' and im.size==(512,768)
hashes=[]
for i in range(24):
    x,y=i%4*128,i//4*128
    cell=im.crop((x,y,x+128,y+128));alpha=cell.getchannel('A')
    bbox=alpha.point(lambda a:255 if a>127 else 0).getbbox()
    assert bbox and bbox[0]>0 and bbox[2]<128 and bbox[1]>0 and bbox[3]==113,(i,bbox)
    assert alpha.getextrema()[0]==0
    hashes.append(hashlib.sha256(cell.tobytes()).hexdigest())
assert len(set(hashes))==24,'Animation cells must be distinct drawings'
assert hashes[12]!=hashes[13]!=hashes[14]!=hashes[15]
src=(root/'Projeto Jogo 2D/Assets/Scripts/SinucaRoom3D.Opponent.cs').read_text()
assert 'OpponentMesh' not in src and 'GetPixel' not in src,'No deformation of original pixels'
assert 'Npc_Ze_Sinuca' in src and 'Sprite[24]' in src
print('PASS: 24 distinct drawings, RGBA transparency, aligned feet, no clipped cells, runtime loads new atlas; no old sprite deformation.')

original=Image.open(root/"Arte/Producao/ZeSinuca/ZeSinuca_Atlas.png").convert("RGBA")
assert im.crop((0,0,512,512)).tobytes()==original.tobytes(),"Original sixteen drawings changed"
print("PASS: original sixteen cells preserved byte for byte.")
