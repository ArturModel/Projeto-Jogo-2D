local root=assert(app.params.root);local atlas=app.open(assert(app.params.source));local source=atlas.cels[1].image
local names={"CasaAvo","CasaSimples_A","CasaSimples_B","CasaSimples_C","Fachada_Mercadinho","Fachada_BarDoZe","Posto_Cobertura","Venda"}
for i,name in ipairs(names) do
  local cx,cy=(i-1)%4,math.floor((i-1)/4)
  local x0,y0=math.floor(cx*source.width/4),math.floor(cy*source.height/2)
  local x1,y1=math.floor((cx+1)*source.width/4)-1,math.floor((cy+1)*source.height/2)-1
  local l,t,r,b=x1,y1,x0,y0
  for y=y0,y1 do for x=x0,x1 do if app.pixelColor.rgbaA(source:getPixel(x,y))>128 then
    l=math.min(l,x);t=math.min(t,y);r=math.max(r,x);b=math.max(b,y)
  end end end
  local factor=math.min(232/(r-l+1),216/(b-t+1));local dw,dh=math.floor((r-l+1)*factor+.5),math.floor((b-t+1)*factor+.5)
  local sprite=Sprite(256,256,ColorMode.RGB);local image=Image(256,256,ColorMode.RGB);local ox,oy=math.floor((256-dw)/2),256-32-dh
  for y=0,dh-1 do for x=0,dw-1 do image:drawPixel(ox+x,oy+y,source:getPixel(l+math.floor(x*(r-l+1)/dw),t+math.floor(y*(b-t+1)/dh))) end end
  sprite.cels[1].image=image;sprite:saveAs(root.."/"..name..".aseprite");sprite:saveCopyAs(root.."/"..name..".png");sprite:close()
end
atlas:close();print("Santa Rita: 8 fachadas exportadas com pivô nos pés")
