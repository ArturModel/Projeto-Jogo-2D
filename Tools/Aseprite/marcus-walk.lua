-- Acabamento nativo: preserva rosto/roupa de Marcus e redesenha apoio, passagem e balanço.
local root=assert(app.params.root)
local atlas=app.open(assert(app.params.source));local src=atlas.cels[1].image
local sheet=Image(576,192,ColorMode.RGB)
local pc=app.pixelColor
local function color(r,g,b) return pc.rgba(r,g,b,255) end
local outline=color(17,19,21);local denim=color(27,58,90);local shade=color(19,40,64)
local light=color(40,75,108);local shoe=color(91,86,68);local sole=color(44,45,40)
local poses={1,1,0,2,4,4,0,5};local bob={0,1,0,-1,0,1,0,-1}
local function line(im,x0,y0,x1,y1,width,c)
  local steps=math.max(math.abs(x1-x0),math.abs(y1-y0),1)
  for i=0,steps do
    local x=math.floor(x0+(x1-x0)*i/steps+.5);local y=math.floor(y0+(y1-y0)*i/steps+.5)
    for dx=-math.floor(width/2),math.ceil(width/2)-1 do im:drawPixel(x+dx,y,c) end
  end
end
local function leg(im,hx,hy,kx,ky,fx,fy,dark,side)
  line(im,hx,hy,kx,ky,6,outline);line(im,kx,ky,fx,fy-3,5,outline)
  line(im,hx,hy,kx,ky,4,dark and shade or denim);line(im,kx,ky,fx,fy-3,3,dark and denim or light)
  for y=fy-3,fy do for x=fx-3,fx+2 do im:drawPixel(x,y,outline) end end
  for y=fy-2,fy-1 do for x=fx-2,fx+1 do im:drawPixel(x,y,dark and sole or shoe) end end
  if side then im:drawPixel(fx-4,fy-1,outline);im:drawPixel(fx-3,fy-1,shoe) end
end
for row=0,2 do
  for y=0,63 do for x=0,63 do sheet:drawPixel(512+x,row*64+y,src:getPixel(x,row*64+y)) end end
  local spr=Sprite(64,64,ColorMode.RGB)
  for f=0,7 do
    if f>0 then spr:newEmptyFrame() end
    local im=Image(64,64,ColorMode.RGB)
    for y=0,41 do for x=0,63 do im:drawPixel(x,y+bob[f+1]+1,src:getPixel(poses[f+1]*64+x,row*64+y)) end end
    if row<2 then
      local phase=f%4;local flip=f>=4
      local supportX=({27,28,29,29})[phase+1];local supportY=({55,55,54,53})[phase+1]
      local swingX=({35,35,34,35})[phase+1];local swingY=({52,50,51,54})[phase+1]
      if flip then supportX=63-supportX;swingX=63-swingX end
      leg(im,flip and 35 or 28,41+bob[f+1],supportX,48,supportX,supportY,false,false)
      leg(im,flip and 28 or 35,41+bob[f+1],swingX,47,swingX,swingY,true,false)
    else
      local nearX=({24,26,30,34,38,35,31,27})[f+1]
      local farX=({38,35,31,27,24,26,30,34})[f+1]
      local lift=({0,1,4,2,0,1,4,2})[f+1]
      leg(im,33,41+bob[f+1],math.floor((33+farX)/2),48,farX,55-(f<4 and lift or 0),true,true)
      leg(im,30,41+bob[f+1],math.floor((30+nearX)/2),48,nearX,55-(f>=4 and lift or 0),false,true)
    end
    if f==0 then spr.cels[1].image=im else spr:newCel(spr.layers[1],f+1,im,Point(0,0)) end
    spr.frames[f+1].duration=.083333
    sheet:drawImage(im,Point(f*64,row*64))
  end
  local tag=spr:newTag(1,8);tag.name=({'walk_down','walk_up','walk_left'})[row+1]
  spr:saveAs(root..'/Marcus_'..tag.name..'.aseprite');spr:saveCopyAs(root..'/Marcus_'..tag.name..'.gif');spr:close()
end
local out=Sprite(576,192,ColorMode.RGB);out.cels[1].image=sheet;out:saveAs(root..'/MarcusWalkFluido.png');out:close();atlas:close()
print('Marcus: 8 poses por direção; contatos nos quadros 0 e 4, sem interpolação de pixels.')
