-- Ciclo limpo: duas pernas completas, nunca sobrepor recortes de pernas anteriores.
local root=assert(app.params.root);local atlas=app.open(assert(app.params.source));local src=atlas.cels[1].image
local pc=app.pixelColor;local function c(r,g,b)return pc.rgba(r,g,b,255)end
local ink=c(18,21,23);local denim=c(30,58,84);local shade=c(22,42,63);local light=c(46,76,103)
local sheet=Image(1088,192,ColorMode.RGB);sheet:clear()
local poses={0,1,2,1,3,4,5,4}
local function stroke(im,x,y,xx,yy,w,col)
 local n=math.max(math.abs(xx-x),math.abs(yy-y),1)
 for j=0,n do local px=math.floor(x+(xx-x)*j/n+.5);local py=math.floor(y+(yy-y)*j/n+.5)
 for d=-math.floor(w/2),math.ceil(w/2)-1 do im:drawPixel(px+d,py,col)end end
end
local function leg(im,hx,hy,kx,ky,fx,fy,far,side)
 stroke(im,hx,hy,kx,ky,7,ink);stroke(im,kx,ky,fx,fy-2,6,ink)
 stroke(im,hx,hy,kx,ky,5,far and shade or denim);stroke(im,kx,ky,fx,fy-2,4,far and shade or denim)
 stroke(im,hx-1,hy+2,kx-1,ky,1,far and denim or light)
 local left=side and fx-4 or fx-3
 for y=fy-2,fy do for x=left,fx+2 do im:drawPixel(x,y,ink)end end
 for x=left+1,fx+1 do im:drawPixel(x,fy-1,far and c(66,63,51) or c(104,96,72))end
end
for row=0,2 do
 local anim=Sprite(64,64,ColorMode.RGB)
 for f=0,16 do
  local idle=f==16;local theta=(f%16)*math.pi/8;local bob=idle and 0 or -math.floor(math.sin(theta)^2+.5)
  local im=Image(64,64,ColorMode.RGB);im:clear()
  for which=1,2 do
   local phase=theta+(which==1 and math.pi or 0);local swing=idle and 0 or math.max(0,-math.sin(phase))
   local hx=row==2 and (which==1 and 33 or 30) or (which==1 and 35 or 28)
   local fx,fy,kx,ky
   if row==2 then
    fx=idle and hx or hx-math.floor(5*math.cos(phase)+.5);fy=55-math.floor(swing*3+.5)
    kx=(hx+fx)/2-swing*2;ky=48+bob-swing
   else
    fx=hx;fy=idle and 55 or 54+math.floor(math.cos(phase)*2+.5)-math.floor(swing*2+.5)
    kx=hx+(which==1 and 1 or -1)*swing;ky=48+bob-swing
   end
   leg(im,hx,41+bob,kx,ky,fx,fy,which==1,row==2)
  end
  -- Rosto, roupa e cintura autorais por cima da raiz das duas pernas.
  local pose=idle and 0 or poses[math.floor(f/2)+1]
  for y=0,41 do for x=0,63 do local v=src:getPixel(pose*64+x,row*64+y)
   if pc.rgbaA(v)>0 then im:drawPixel(x,y+bob,v)end
  end end
  sheet:drawImage(im,Point(f*64,row*64))
  if f<16 then
   if f>0 then anim:newEmptyFrame()end
   if f==0 then anim.cels[1].image=im else anim:newCel(anim.layers[1],f+1,im,Point(0,0))end
   anim.frames[f+1].duration=.05
  end
 end
 anim:newTag(1,16).name=({'walk_down','walk_up','walk_left'})[row+1]
 anim:saveAs(root..'/Marcus_'..({'walk_down','walk_up','walk_left'})[row+1]..'.aseprite')
 anim:saveCopyAs(root..'/Marcus_'..({'walk_down','walk_up','walk_left'})[row+1]..'.gif');anim:close()
end
local out=Sprite(1088,192,ColorMode.RGB);out.cels[1].image=sheet;out:saveAs(root..'/MarcusWalkFluido.png');out:close();atlas:close()
