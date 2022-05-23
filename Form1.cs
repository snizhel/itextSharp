
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.IO;
using System.Windows.Forms;

namespace iTextSharpExemplo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            string arquivoTeste = "..\\..\\PDFs\\Teste.pdf";

            if (File.Exists(arquivoTeste))
            {
                txtUrl.Text = System.IO.Path.GetFullPath(arquivoTeste);

                //Classe para leitura de PDF
                PdfReader reader = new PdfReader(arquivoTeste);

                //Extraí o texto da página 1 do arquivo PDF
                rtbTexto.Text = PdfTextExtractor.GetTextFromPage(reader, 1);

                //Fecha o leitor de PDF
                reader.Close();
            }
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "(*.pdf)|*.pdf";

            if (open.ShowDialog() == DialogResult.OK)
            {
                txtUrl.Text = open.FileName;

                //Classe para leitura de PDF
                PdfReader reader = new PdfReader(open.FileName);

                //Extraí o texto da página 1 do arquivo PDF
                rtbTexto.Text = PdfTextExtractor.GetTextFromPage(reader, 1);

                //Fecha o leitor de PDF
                reader.Close();
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "Test";
            save.Filter = "(*.pdf)|*.pdf";
            save.ShowDialog();

            if (save.FileName != "")
            {
                //Cria um documento com margem A4
                Document document = new Document(PageSize.A4);
                document.PageCount = 1; //Número da página

                //Define como o arquivo será tratado
                FileStream fs = new FileStream(save.FileName, FileMode.Create, FileAccess.Write, FileShare.None);

                //Define a instância para colocar conteúdo e salvar o PDF
                PdfWriter writer = PdfWriter.GetInstance(document, fs);

                document.Open();

               
                Font f_tr = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16, BaseColor.DARK_GRAY);

                
                Chunk c1 = new Chunk(rtbTexto.Text, f_tr);

               
                Paragraph p1 = new Paragraph(c1);
               
                p1.Alignment = Element.ALIGN_JUSTIFIED;
                p1.FirstLineIndent = 40;
                p1.IndentationLeft = 30;

               
                document.Add(p1);

                MarcaDAgua(writer);
                Coordenadas(writer); 
                document.Add(Tabela());
                ImagemRodape(writer, document.PageNumber);

                //document.NewPage();
                //document.PageCount = 2;

                //Contato(writer);

                document.Close();

                fs.Close();
                writer.Close();
            }
        }

        private void MarcaDAgua(PdfWriter writer)
        {
            
            PdfContentByte canvas = writer.DirectContent;

          
            BaseFont f_tr = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

           
            canvas.SetFontAndSize(f_tr, 60);

           
            string watermarkText = "Snizhel";

            
            PdfGState gstate = new PdfGState { FillOpacity = 0.35f, StrokeOpacity = 0.3f };
            canvas.SaveState();
            canvas.SetGState(gstate);
            canvas.SetColorFill(BaseColor.BLACK);

           
            canvas.BeginText();
            canvas.ShowTextAligned(Element.ALIGN_CENTER, watermarkText, 320, 420, 20);
            canvas.EndText();
            canvas.RestoreState();
        }

        private void Coordenadas(PdfWriter writer)
        {
            BaseFont f_tr = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            PdfContentByte canvas = writer.DirectContent;
            canvas.SetFontAndSize(f_tr, 8);

            canvas.BeginText();

           
            for (int y = 1; y <= 829; y += 12)
            {
                canvas.SetTextMatrix(10, y);
                canvas.ShowText("Y: " + y.ToString());
            }

            //Coordenada horizontal
            for (int x = 35; x <= 560; x += 25) 
            {
                canvas.SetTextMatrix(x, 829);
                canvas.ShowText("X: " + x.ToString());
            }

            canvas.EndText();
        }

        private PdfPTable Tabela()
        {
           
            PdfPTable table = new PdfPTable(3);
            table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.SpacingBefore = 300f;

            Font f_tr = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16);
            Phrase p1 = new Phrase("Tabela", f_tr);

            PdfPCell cell = new PdfPCell(p1);
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 4;
            cell.HorizontalAlignment = 1; // 0 = esquerda, 1 = centro, 2 = direita
            cell.Padding = 10f;
            table.AddCell(cell);

            table.AddCell("Coluna 1");
            table.AddCell("Coluna 2");
            table.AddCell("Coluna 3");

            table.AddCell("Texto");
            table.AddCell("Texto");
            table.AddCell("Texto");

            table.AddCell("Texto");
            table.AddCell("Texto");
            table.AddCell("Texto");

            table.AddCell("Texto");
            table.AddCell("Texto");
            table.AddCell("Texto");

            table.AddCell("Texto");
            table.AddCell("Texto");
            table.AddCell("Texto");

            float[] totalWidth = new float[3];

            
            totalWidth[0] = 10; // 0,14" x 72 user units = 10 
            totalWidth[1] = 15; // 0,21" x 72 user units = 15 
            totalWidth[2] = 15;

            table.SetTotalWidth(totalWidth); 
            table.DefaultCell.FixedHeight = 30; 
            table.DefaultCell.HorizontalAlignment = 1;

            return table;
        }

        private void ImagemRodape(PdfWriter writer, int numberPage)
        {
            BaseFont f_tr = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            PdfContentByte canvas = writer.DirectContent;

           
            Image image1 = Image.GetInstance("..\\..\\Imagens\\imagem1.png");
            image1.SetAbsolutePosition(40, 1); //Horizontal, vertical
            image1.ScaleAbsoluteHeight(60);
            image1.ScaleAbsoluteWidth(60);

            canvas.AddImage(image1);

          
            canvas.MoveTo(40, 60);
            canvas.LineTo(575, 60);
            canvas.SetLineWidth(2);
            canvas.Stroke();

         
            PdfTemplate tmpFooter = canvas.CreateTemplate(540, 70);
            canvas.AddTemplate(tmpFooter, 50, 1);
            tmpFooter.SetFontAndSize(BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false), 5);
            tmpFooter.BeginText();
            tmpFooter.SetFontAndSize(f_tr, 8);
            tmpFooter.ShowTextAligned(1, "TextoTextoTextoTextoTextoTextoTextoTexto", 240, 30, 0);
            tmpFooter.ShowTextAligned(2, string.Format("Página {0}", numberPage), 520, 10, 0);
            tmpFooter.EndText();
        }

        private void Contato(PdfWriter writer)
        {
            BaseFont f_tr1 = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            PdfContentByte canvas = writer.DirectContent;
            canvas.SetFontAndSize(f_tr1, 15);

            canvas.BeginText();
            canvas.ShowTextAligned(0, "E-mail:fangireking789@gmail.com", 180f, 600f, 10f);
            canvas.ShowTextAligned(0, "GitHub: https://github.com/snizhel", 181f, 580f, 10f);
            canvas.EndText();
        }
    }
}