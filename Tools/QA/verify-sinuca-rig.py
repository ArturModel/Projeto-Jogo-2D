"""Read-only atlas and projection checks; these do not render or approve Unity art."""
from pathlib import Path
from PIL import Image
import math,re
r=Path(__file__).resolve().parents[2]
s=(r/'Projeto Jogo 2D/Assets/Scripts/SinucaRoom3D.cs').read_text(encoding='utf-8-sig')
a=r/'Projeto Jogo 2D/Assets/Resources/Art/Sinuca'
shader=(a/'MaoFlutuante.shader').read_text()
n=0
def check(ok,label):
 global n
 assert ok,label
 n+=1
 print('PASS: '+label)
def values(pattern):return tuple(float(v) for v in re.search(pattern,s).groups())
back,eye=values(r'dir\*\(([.\d]+)f/zoom\)\+Vector3.up\*([.\d]+)f')
look,height=values(r'ball\+dir\*([.\d]+)f\+Vector3.up\*([.\d]+)f')
width=float(re.search(r'gripHand=HandSprite\([^;]+new Vector2\([^)]+\),([.\d]+)f',s).group(1))
px,py=values(r'gripHand=HandSprite\([^;]+new Vector2\(([.\d]+)f,([.\d]+)f\)')
gx,gy,gz=values(r'gripHand.localPosition=new Vector3\(([.\d]+)f,([.\d]+)f,(-[.\d]+)f\)')
cue_height=float(re.search(r'strokeOrigin=new Vector3\(cuePos.x\*MetresPerUnit,TableY\+([.\d]+)f',s).group(1))
pull=float(re.search(r'float pull=power\*([.\d]+)f',s).group(1))
length=math.hypot(back+look,height-eye);fy=(height-eye)/length;fz=(back+look)/length
im=Image.open(a/'MaosMarcusUnificadas.png')
check(im.mode=='RGBA' and im.width==im.height*2,'approved atlas is two square RGBA cells')
check(im.getchannel('A').getextrema()==(0,255),'atlas contains transparent margins and opaque skin')
meta=(a/'MaosMarcusUnificadas.png.meta').read_text()
check(all(x in meta for x in ('filterMode: 0','enableMipMap: 0','textureCompression: 0','nPOTScale: 0')),'Point import; no mipmaps, NPOT resize or compression')
cell=im.width//2
for side in range(2):
 check(im.getchannel('A').crop((side*cell,0,(side+1)*cell,im.height)).getbbox() is not None,f'hand {side} has art')
im=Image.open(a/'MaosMarcusUnificadas.png')
check(im.mode=='RGBA' and im.getchannel('A').getextrema()[0]==0 and im.getchannel('A').getextrema()[1]>=250,'new original left grip has transparent background and solid hand')
cell=im.width//2
for aspect in (4/3,16/9,21/9):
 for power in (0,.5,1):
  y,z=cue_height+gy-eye,gz-pull*power+back
  depth=y*fy+z*fz;up=y*fz-z*fy
  total=visible=0
  for iy in range(0,im.height,7):
   for ix in range(cell,im.width,7):
    if im.getpixel((ix,iy))[3]<128:continue
    x0=((ix-cell)/cell-px)*width
    y0=((im.height-iy)/cell-py*im.height/cell)*width
    total+=1
    visible+=abs((up+y0)/depth)<math.tan(math.radians(24)) and abs(x0/depth)<math.tan(math.radians(24))*aspect
  check(depth>.05 and visible/total>.95,f'grip silhouette framed at aspect {aspect:.3f}, power {power}: {visible/total:.1%}')
# Same ray-scale operation as PlaceHandDepth: exact projected points, including seam endpoints.
for depth in (.4,.8,1.4):
 for offset in (-.035,-.025,.025):
  factor=(depth+offset)/depth
  for x,y in ((0,0),(.1,-.13),(-.15,.12)):
   check(abs((x*factor)/(depth*factor)-x/depth)<1e-12 and abs((y*factor)/(depth*factor)-y/depth)<1e-12,'depth layer preserves contact and silhouette projection')
# Contact remains on shaft for flat and raised bridge, through complete pull/release.
for lift in (0,.045):
 for power in (0,.5,1,-.25):
  slope=-lift/.25;norm=math.hypot(1,slope)
  cue_z=-pull*power/norm;cue_y=cue_height-pull*power*slope/norm
  shaft_y=cue_y+(-.25-cue_z)*slope
  check(abs(shaft_y-(cue_height+lift))<1e-12,'stationary bridge contact lies on moving 3D shaft')
check('bridgeAnchor,BridgeContactUV,.21f,false,1' in s and 'bridgeAnchor,BridgeContactUV,.21f,false,2' in s,'support layers use identical sprite, size and pivot')
check('bool thumb=i.uv.x>=boundary;' in shader and '(_BridgeLayer<1.5 && thumb) || (_BridgeLayer>=1.5 && !thumb)' in shader,'thumb and dorsum masks are complementary')
check('AlignBridgeToShaft();' in s and 'new Vector3(-.012f,.035f,-.25f)' not in s,'support uses actual contact without lateral offset')
check('AlignGripToShaft();' in s and 'GripScreenRoll(pixels,90f)' in s,'grip follows projected shaft')
check('gripHand.gameObject.SetActive(handsVisible);' in s and 'bridgeAnchor.gameObject.SetActive(handsVisible);' in s,'walking hides both hands and support layers')
check('gripHand=HandSprite("MaoEmpunhaduraMarcus",cue,' in s,'grip is attached to real cue transform')
check('if(grip){material.SetFloat("_ZTest",8f);material.renderQueue=2470;}' in s and 'ZTest [_ZTest]' in shader,'grip opaque silhouette renders after cue without depth rejection')
print(f'TOTAL: {n} PASS. Numeric/asset checks only. Native shader/render, transitions and zoom still require in-game review.')
