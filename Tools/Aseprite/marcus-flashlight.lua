-- Pose equipada sobre os mesmos oito apoios. Lente e corpo compartilham âncoras 64 px.
local src=app.open(assert(app.params.source)); local base=src.cels[1].image
local root=assert(app.params.root); local sheet=Image(576,192,ColorMode.RGB)
local pc=app.pixelColor
local function c(r,g,b) return pc.rgba(r,g,b,255) end
local ink=c(17,19,21);local shirt=c(28,32,35);local edge=c(43,47,48)
local skin=c(103,68,44);local skinLight=c(130,90,57)
local metal=c(47,52,56);local silver=c(150,158,158);local lens=c(245,225,151)
local function rect(im,x,y,w,h,col) for j=y,y+h-1 do for i=x,x+w-1 do im:drawPixel(i,j,col) end end end
local function line(im,x,y,xx,yy,w,col)
 local n=math.max(math.abs(xx-x),math.abs(yy-y),1)
 for k=0,n do rect(im,math.floor(x+(xx-x)*k/n+.5)-math.floor(w/2),math.floor(y+(yy-y)*k/n+.5),w,1,col) end
end
for row=0,2 do
 local anim=Sprite(64,64,ColorMode.RGB)
 for f=0,8 do
  local im=Image(64,64,ColorMode.RGB)
  for y=0,63 do for x=0,63 do im:drawPixel(x,y,base:getPixel(f*64+x,row*64+y)) end end
  local bob=f<8 and ({0,1,0,-1,0,1,0,-1})[f+1]+1 or 0
  if row<2 then
   -- Retira somente o braço exterior direito, preservando rosto, tronco e pernas.
   rect(im,39,30+bob,5,13,0)
   line(im,38,30+bob,42,34+bob,6,ink);line(im,38,30+bob,42,34+bob,4,shirt)
   line(im,42,34+bob,40,39+bob,5,ink);line(im,42,34+bob,40,39+bob,3,edge)
   if row==0 then
    rect(im,38,38+bob,5,9,ink);rect(im,39,39+bob,3,6,metal)
    rect(im,38,44+bob,5,2,silver);rect(im,39,46+bob,3,1,lens)
   else
    rect(im,38,28+bob,5,12,ink);rect(im,39,30+bob,3,9,metal)
    rect(im,38,29+bob,5,2,silver);rect(im,39,28+bob,3,1,lens)
   end
   rect(im,39,38+bob,3,3,skin);im:drawPixel(40,38+bob,skinLight)
  else
   rect(im,27,30+bob,6,12,shirt)
   line(im,32,30+bob,28,35+bob,6,ink);line(im,32,30+bob,28,35+bob,4,shirt)
   line(im,28,35+bob,23,36+bob,5,ink);line(im,28,35+bob,23,36+bob,3,edge)
   rect(im,16,34+bob,9,5,ink);rect(im,18,35+bob,6,3,metal)
   rect(im,17,34+bob,2,5,silver);rect(im,16,35+bob,1,3,lens)
   rect(im,22,35+bob,3,3,skin);im:drawPixel(22,35+bob,skinLight)
  end
  sheet:drawImage(im,Point(f*64,row*64))
  if f>0 then anim:newEmptyFrame() end
  anim:newCel(anim.layers[1],f+1,im,Point(0,0));anim.frames[f+1].duration=.083333
 end
 anim:newTag(1,8).name=({'lanterna_baixo','lanterna_cima','lanterna_esquerda'})[row+1]
 anim:saveAs(root..'/Marcus_lanterna_'..row..'.aseprite');if app.params.preview then anim:saveCopyAs(app.params.preview..'/Marcus_lanterna_'..row..'.gif') end;anim:close()
end
local out=Sprite(576,192,ColorMode.RGB);out.cels[1].image=sheet;out:saveAs(root..'/MarcusLanterna.png');out:close();src:close()
print('Marcus equipado: 8 apoios + repouso em 3 direções; direita por um único espelhamento.')
