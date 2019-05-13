openInRoot(const char* path)
{
	TGraph *g = new TGraph(path,"%lg %lg");
	g->SetFillColor(kAzure-5);
	g->DrawClone("AB");
}