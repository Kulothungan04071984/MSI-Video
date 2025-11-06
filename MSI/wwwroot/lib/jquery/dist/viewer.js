// Path to your PDF file
const path = document.getElementById("pageData");
const pdfPath = path.dataset.message; // Replace with your PDF file path

// PDF.js setup
pdfjsLib.GlobalWorkerOptions.workerSrc =
    "https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.14.305/pdf.worker.min.js";

const canvas = document.getElementById("pdfCanvas");
const context = canvas.getContext("2d");

// Load the PDF
pdfjsLib.getDocument(pdfPath).promise.then(pdf => {
    console.log("PDF loaded");

    // Get first page
    pdf.getPage(1).then(page => {
        console.log("Page loaded");

        // Fit canvas to page
        const viewport = page.getViewport({ scale: 1 });
        const scale = Math.min(
            canvas.clientWidth / viewport.width || 1,
            canvas.clientHeight / viewport.height || 1
        );

        const fittedViewport = page.getViewport({ scale });

        canvas.width = fittedViewport.width;
        canvas.height = fittedViewport.height;

        const renderContext = {
            canvasContext: context,
            viewport: fittedViewport
        };
        page.render(renderContext);
    });
});
